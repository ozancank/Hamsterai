import 'dart:async';

import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/scan/service/scan_service.dart';
import 'package:mobile/pages/similarQuestions/model/get_similar_question_response_model.dart';

class SimilarQuestionController extends GetxController {
  late ScanService _scanService;
  Timer? timer;
  RxList<dynamic> questionList = [].obs;
  var loading = false.obs;

  @override
  void onReady() async {
    await getSimilarQuestions();
    super.onReady();
  }

  @override
  void onInit() async {
    _scanService = ScanService(NetworkManager.instance.dio);
    await getSimilarQuestions();
    timer = Timer.periodic(const Duration(seconds: 30), (timer) {
      getSimilarQuestions();
    });
    super.onInit();
  }

  Future<void> getSimilarQuestions() async {
    loading.value = true;
    var today = DateTime.now();
    var startDate = today.subtract(const Duration(days: 2));
    var getQuestionModel = await _scanService.getSimilarQuestions({
      "lessonId": 0,
      "startDate": startDate.toIso8601String(),
      "endDate": today.toIso8601String(),
    });
    if (getQuestionModel is GetSimilarQuestionResponseModel) {
      EasyLoading.dismiss();
      loading.value = false;
      questionList.value = getQuestionModel.items;
    } else if (getQuestionModel is ErrorModel) {
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
