package com.example.beertrackerapp.realmsync

import io.realm.RealmObject
import io.realm.annotations.PrimaryKey
import io.realm.annotations.RealmField
import org.bson.types.Decimal128
import org.bson.types.ObjectId
import java.util.*



open class Beverage(
    @RealmField("_id") @PrimaryKey var id: ObjectId = ObjectId(),
    var BeverageName: String = "",
    var ABV: Decimal128 = Decimal128(0),
    var Description: String? = null,
    var LastUpdate: Date? = null,
    var Category: Long = 0,
): RealmObject()
