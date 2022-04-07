package com.example.beertrackerapp.ui

import android.app.AlertDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.Toolbar
import androidx.recyclerview.widget.DividerItemDecoration
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.beertrackerapp.R
import com.example.beertrackerapp.adapters.BeverageAdapter
import com.example.beertrackerapp.realmsync.Beverage
import io.realm.Realm
import io.realm.RealmConfiguration
import io.realm.kotlin.where
import io.realm.mongodb.sync.SyncConfiguration
import org.bson.types.Decimal128
import java.math.BigDecimal

class BeverageActivity : AppCompatActivity() {
    private lateinit var userRealm: Realm
    private lateinit var config: RealmConfiguration
    private lateinit var recyclerView: RecyclerView

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_beverage)

        val toolbar = findViewById<View>(R.id.drink_menu) as Toolbar
        setSupportActionBar(toolbar)

        val fab = findViewById<View>(R.id.floating_action_button)
        fab.setOnClickListener { (onFabClicked()) }

        recyclerView = findViewById(R.id.drink_list)
        recyclerView.layoutManager = LinearLayoutManager(this)
        recyclerView.setHasFixedSize(true)
        recyclerView.addItemDecoration(DividerItemDecoration(this, DividerItemDecoration.VERTICAL))
    }
    /**
     *  On start, open a realm with a partition ID equal to the user ID.
     *  Query the realm for Task objects, sorted by a stable order that remains
     *  consistent between runs, and display the collection using a recyclerView.
     */
    override fun onStart() {
        super.onStart()
        val user = realmApp.currentUser()
        if (user == null) {
            startActivity(Intent(this, LoginActivity::class.java))
        }
        else {
            val partition = user.id
            config = SyncConfiguration.Builder(user, partition).build()
            Realm.getInstanceAsync(config, object : Realm.Callback() {
                override fun onSuccess(realm: Realm) {
                    this@BeverageActivity.userRealm = realm
                    recyclerView.adapter = BeverageAdapter(realm.where<Beverage>().sort("id").findAllAsync(), config)
                }
            })
        }
    }

    /**
     * Add buttons to the task menu.
     */
    override fun onCreateOptionsMenu(menu: Menu): Boolean {
        menuInflater.inflate(R.menu.beverage_menu, menu)
        return true
    }
    /**
     *  Decides actions for all buttons on the task menu.
     *  When "log out" is tapped, logs out the user and brings them back to the login screen.
     */
    override fun onOptionsItemSelected(menuItem: MenuItem): Boolean {
        return when (menuItem.itemId) {
            R.id.action_logout -> {
                realmApp.currentUser()?.logOutAsync {
                    if (it.isSuccess) {
                        Log.v(TAG(), "user logged out")
                        startActivity(Intent(this, LoginActivity::class.java))
                    } else {
                        Log.e(TAG(), "log out failed! Error: ${it.error}")
                    }
                }
                true
            }
            else -> {
                super.onOptionsItemSelected(menuItem)
            }
        }
    }

    /**
     *  Creates a popup that allows the user to insert a task into the realm.
     */
    private fun onFabClicked() {
        val input = EditText(this)
        val dialogBuilder = AlertDialog.Builder(this)
        dialogBuilder.setMessage("Enter beverage name:")
            .setCancelable(true)
            .setPositiveButton("Create") { dialog, _ ->
                run {
                    dialog.dismiss()
                    val beverage = Beverage()
                    beverage.BeverageName = input.text.toString()
                    beverage.Description = input.text.toString()
                    var _ABV: Decimal128 = Decimal128(0)
                    beverage.ABV = _ABV
                    beverage.Category = 1

                    userRealm.executeTransactionAsync { realm ->
                        realm.insert(beverage)
                    }
                }
            }
            .setNegativeButton("Cancel") { dialog, _ ->
                dialog.cancel()
            }
        val dialog = dialogBuilder.create()
        dialog.setView(input)
        dialog.setTitle("Add Beverage")
        dialog.show()
        input.requestFocus()
    }

    /**
     * Ensure the user realm closes when the activity ends.
     */
    override fun onDestroy() {
        super.onDestroy()
        userRealm.close()
        recyclerView.adapter = null
    }
}