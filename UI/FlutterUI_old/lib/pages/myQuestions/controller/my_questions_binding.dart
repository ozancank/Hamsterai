import 'package:get/get.dart';

import 'my_questions_controller.dart';

class MyQuestionsBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<MyQuestionsController>(() => MyQuestionsController());
  }
}
