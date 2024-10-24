import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/homeworks/model/homework_model.dart';
import 'package:mobile/pages/homeworks/service/homework_service.dart';

class HomeworkController extends GetxController {
  late HomeworkService _homeworkService;
  HomeWorkModel? homeWorkModel;
  RxList<dynamic> homeWorkList = [].obs;
  var loading = false.obs;
  var lessonId = 0.obs;
  @override
  void onInit() {
    _homeworkService = HomeworkService(NetworkManager.instance.dio);
    getHomeworks();
    super.onInit();
  }

  Future<void> getHomeworks() async {
    loading.value = true;
    var getHomeWork = await _homeworkService.getHomeworks(lessonId.value);
    if (getHomeWork is HomeWorkModel) {
      loading.value = false;
      homeWorkModel = getHomeWork;
      homeWorkList.value = getHomeWork.items;
    } else if (getHomeWork is ErrorModel) {
      loading.value = false;
    }
  }
}
