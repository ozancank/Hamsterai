import 'package:get/get.dart';
import 'package:mobile/pages/questionDetails/controller/question_detail_controller.dart';

class QuestionDetailBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<QuestionDetailController>(() => QuestionDetailController());
  }
}
