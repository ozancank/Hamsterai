import 'package:get/get.dart';

class HomePageController extends GetxController {
  var notifications = <String>[].obs;
  void addNotification(String notification) {
    notifications.add(notification);
  }
}
