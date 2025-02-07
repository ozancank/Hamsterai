import 'package:get/get.dart';
import 'package:mobile/pages/similarQuestions/controller/similar_question_controller.dart';

class SimilarQuestionBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<SimilarQuestionController>(() => SimilarQuestionController());
  }
}
