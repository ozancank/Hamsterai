import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/statistics/model/gain_model.dart';
import 'package:mobile/pages/statistics/service/statistics_service.dart';

class StatisticsController extends GetxController {
  late StatisticsService _service;
  Rx<LessonDataModel> studentGain = LessonDataModel(
    forLessons: {},
    forGains: {},
    forLessonGains: {},
  ).obs;
  var loading = false.obs;

  @override
  void onInit() async {
    _service = StatisticsService(NetworkManager.instance.dio);
    await getStudentGain();
    super.onInit();
  }

  Future<void> getStudentGain() async {
    loading.value = true;
    var getStudentGain = await _service.getStudentGain();
    if (getStudentGain is LessonDataModel) {
      Future.delayed(const Duration(seconds: 3), () {
        loading.value = false;
        studentGain.value = getStudentGain;
      });
    } else if (getStudentGain is ErrorModel) {
      loading.value = false;
      // return QuickAlert.show(
      //   context: context,
      //   type: QuickAlertType.error,
      //   text: getQuestionModel.message,
      //   confirmBtnText: 'Tamam',
      //   confirmBtnColor: MyColors.redColor,
      //   confirmBtnTextStyle: Theme.of(context)
      //       .textTheme
      //       .bodyMedium!
      //       .copyWith(color: Colors.white),
      //   title: 'Hata',
      // );
    }
  }
}
