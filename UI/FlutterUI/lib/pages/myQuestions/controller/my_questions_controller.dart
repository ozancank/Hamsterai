import 'dart:async';

import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';
import 'package:mobile/pages/scan/service/scan_service.dart';

class MyQuestionsController extends GetxController {
  late ScanService _scanService;
  Timer? timer;
  RxList<dynamic> questionList = [].obs;
  var loading = false.obs;

  @override
  void onInit() async {
    _scanService = ScanService(NetworkManager.instance.dio);
    await getQuestions();
    super.onInit();
  }

  Future<void> getQuestions() async {
    loading.value = true;
    var today = DateTime.now();
    var startDate = today.subtract(const Duration(days: 2));
    var getQuestionModel = await _scanService.getQuestions({
      "lessonId": 0,
      // "startDate": startDate.toIso8601String(),
      // "endDate": today.toIso8601String(),
      "startDate": null,
      "endDate": null,
    });
    if (getQuestionModel is GetQuestionsResponseModel) {
      EasyLoading.dismiss();
      loading.value = false;
      questionList.value = getQuestionModel.items;
    } else if (getQuestionModel is ErrorModel) {
      loading.value = false;
      EasyLoading.dismiss();
    }
  }

  Future<void> readQuestion(String questionId) async {
    loading.value = true;
    var getQuestionModel = await _scanService.readQuestion(questionId);
    if (getQuestionModel) {
      EasyLoading.dismiss();
      loading.value = false;
    } else if (getQuestionModel == false) {
      loading.value = false;
      EasyLoading.dismiss();
    }
  }

  @override
  void onClose() {
    timer?.cancel();
    super.onClose();
  }
}
