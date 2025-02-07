import 'package:get/get.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';

class QuizBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<QuizController>(() => QuizController());
  }
}
