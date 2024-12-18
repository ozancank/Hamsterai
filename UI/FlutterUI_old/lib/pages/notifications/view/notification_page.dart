import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/home/controller/home_page_controller.dart';
import 'package:mobile/styles/colors.dart';

class NotificationPage extends StatefulWidget {
  const NotificationPage({super.key});

  @override
  State<NotificationPage> createState() => _NotificationPageState();
}

class _NotificationPageState extends State<NotificationPage> {
  final HomePageController homePageController = Get.find<HomePageController>();
  bool isActive = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Bildirimler'),
      backgroundColor: const Color(0xFFF6F7F8),
      body: Obx(() {
        if (homePageController.notifications.isEmpty) {
          return const Center(
            child: Text('Henüz bildirim yok'),
          );
        }
        return SingleChildScrollView(
          child: Column(
            children: homePageController.notifications.map((notification) {
              return GestureDetector(
                onTap: () {
                  HapticFeedback.mediumImpact();
                  setState(() {
                    isActive = !isActive;
                  });
                },
                child: Container(
                  margin: EdgeInsets.symmetric(
                      vertical: context.dynamicHeight * 0.01,
                      horizontal: context.dynamicHeight * 0.02),
                  padding: EdgeInsets.symmetric(
                      horizontal: context.dynamicWidth * 0.01, vertical: 10),
                  decoration: BoxDecoration(
                    color: Colors.white,
                    borderRadius: BorderRadius.circular(40),
                  ),
                  child: Row(
                    children: [
                      Container(
                        alignment: Alignment.center,
                        padding: const EdgeInsets.all(10),
                        decoration: BoxDecoration(
                          shape: BoxShape.circle,
                          color: isActive
                              ? MyColors.grayColor
                              : MyColors.primaryColor,
                        ),
                        child: SvgPicture.asset(
                          AssetsConstant.bell,
                        ),
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          notification,
                          style: Theme.of(context)
                              .textTheme
                              .headlineLarge!
                              .copyWith(
                                fontWeight: isActive
                                    ? FontWeight.w300
                                    : FontWeight.w700,
                                color: MyColors.darkBlue,
                                fontSize: 14,
                              ),
                        ),
                      ),
                      const SizedBox(width: 10),
                    ],
                  ),
                ),
              );
            }).toList(),
          ),
        );
      }),
    );
  }
}


// import 'package:flutter/material.dart';
// import 'package:flutter/services.dart';
// import 'package:flutter_svg/svg.dart';
// import 'package:get/get.dart';
// import 'package:mobile/core/constants/assets_constant.dart';
// import 'package:mobile/core/extensions/size_extension.dart';
// import 'package:mobile/pages/common/common_appbar.dart';
// import 'package:mobile/pages/home/controller/home_page_controller.dart';
// import 'package:mobile/styles/colors.dart';

// class NotificationPage extends StatefulWidget {
//   const NotificationPage({super.key});

//   @override
//   State<NotificationPage> createState() => _NotificationPageState();
// }

// class _NotificationPageState extends State<NotificationPage> {
//   final HomePageController homePageController = Get.find<HomePageController>();
//   bool isActive = false;
//   @override
//   Widget build(BuildContext context) {
//     return Scaffold(
//       appBar: const CommonAppbar(title: 'Bildirimler'),
//       backgroundColor: const Color(0xFFF6F7F8),
//       body: Obx(() {
//         if (homePageController.notifications.isEmpty) {
//           return const Center(
//             child: Text('Henüz bildirim yok'),
//           );
//         }

//         return SingleChildScrollView(
//           child: Column(
//             children: [
//               InkWell(
//                 onTap: () {
//                   HapticFeedback.mediumImpact();
//                 },
//                 child: Container(
//                   margin: EdgeInsets.symmetric(
//                       vertical: context.dynamicHeight * 0.01,
//                       horizontal: context.dynamicHeight * 0.02),
//                   padding: EdgeInsets.symmetric(
//                       horizontal: context.dynamicWidth * 0.01, vertical: 10),
//                   decoration: BoxDecoration(
//                     color: Colors.white,
//                     borderRadius: BorderRadius.circular(40),
//                   ),
//                   child: Row(
//                     children: [
//                       Container(
//                         alignment: Alignment.center,
//                         padding: const EdgeInsets.all(10),
//                         decoration: BoxDecoration(
//                           shape: BoxShape.circle,
//                           color: isActive
//                               ? MyColors.grayColor
//                               : MyColors.primaryColor,
//                         ),
//                         child: SvgPicture.asset(
//                           AssetsConstant.bell,
//                         ),
//                       ),
//                       const Spacer(),
//                       Text(
//                         'Geometri sorunuz çözüldü.',
//                         style: Theme.of(context)
//                             .textTheme
//                             .headlineLarge!
//                             .copyWith(
//                               fontWeight:
//                                   isActive ? FontWeight.w300 : FontWeight.w700,
//                               color: MyColors.darkBlue,
//                               fontSize: 14,
//                             ),
//                       ),
//                       const Spacer(),
//                       const Spacer(),
//                     ],
//                   ),
//                 ),
//               ),
//             ],
//           ),
//         );
//       }),
//     );
//   }
// }
