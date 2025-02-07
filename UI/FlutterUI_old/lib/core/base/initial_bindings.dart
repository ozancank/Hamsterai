import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:get/get.dart';

class InitialBinding implements Bindings {
  @override
  void dependencies() {
    Get.put(Dio());
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      // var token = LocaleManager.instance.getStringValue(PreferencesKeys.TOKEN);
      // token != '' ? Get.put(MyQuestionsController()) : null;
    });
  }
}
