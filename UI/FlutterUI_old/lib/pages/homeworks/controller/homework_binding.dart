import 'package:get/get.dart';
import 'package:mobile/pages/homeworks/controller/homework_controller.dart';

class HomeworkBinding implements Bindings {
  @override
  void dependencies() {
    Get.lazyPut<HomeworkController>(() => HomeworkController());
  }
}
