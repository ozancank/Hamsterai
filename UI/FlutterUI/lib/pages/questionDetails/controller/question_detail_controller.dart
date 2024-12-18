import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/questionDetails/service/question_detail_service.dart';

class QuestionDetailController extends GetxController {
  late QuestionDetailService _questionDetailService;

  var loading = false.obs;

  @override
  void onInit() {
    _questionDetailService = QuestionDetailService(NetworkManager.instance.dio);
    super.onInit();
  }

  Future<void> reSendQuestion(String questionId) async {
    EasyLoading.show(status: 'Soru yeniden gönderiliyor...');
    loading.value = true;
    var reSendQuestion =
        await _questionDetailService.reSendQuestion(questionId);
    if (reSendQuestion) {
      EasyLoading.showSuccess('Soru başarıyla tekrar gönderildi.');
      loading.value = false;
    } else if (reSendQuestion == false) {
      loading.value = false;
      EasyLoading.showError('Soru gönderilirken bir hata oluştu.');
    }
  }

  Future<void> updateQuestionText(
      String questionId, String questionText) async {
    EasyLoading.show(status: 'Soru gönderiliyor...');
    loading.value = true;
    var updateQuestionText = await _questionDetailService.updateQuestionText(
        questionId, questionText);
    if (updateQuestionText) {
      EasyLoading.showSuccess('Soru başarıyla gönderildi.');
      Get.back();
      loading.value = false;
    } else if (updateQuestionText == false) {
      loading.value = false;
      EasyLoading.showError('Soru gönderilirken bir hata oluştu.');
    }
  }
}
