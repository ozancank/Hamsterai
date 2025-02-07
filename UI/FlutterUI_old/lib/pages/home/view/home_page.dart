import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/controllers/bottom_nav_controller.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/module/custom_button_navigation_bar.dart';
import 'package:mobile/pages/category/view/category_page.dart';
import 'package:mobile/pages/profile/controller/profile_controller.dart';
import 'package:mobile/pages/profile/view/profile_page.dart';
import 'package:mobile/styles/colors.dart';

class HomePage extends StatelessWidget {
  HomePage({super.key});
  final BottomNavController controller = Get.put(BottomNavController());
  final profilController = Get.put(ProfileController());
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          const CategoryPage(),
          Positioned(
            bottom: 0,
            right: context.width * 0.1,
            child: GestureDetector(
              onTap: () {
                print(profilController.userModel);
                Get.to(() => const ProfilePage());
              },
              child: Container(
                height: context.height * 0.15,
                width: context.width * 0.15,
                alignment: Alignment.center,
                decoration: const BoxDecoration(
                  color: MyColors.primaryColor,
                  shape: BoxShape.circle,
                ),
                child: SvgPicture.asset(
                  AssetsConstant.manageAccounts,
                  height: 24,
                  width: 24,
                  color: Colors.white,
                ),
              ),
            ),
          ),
        ],
      ),
    );
    // return Scaffold(
    //   body: Obx(() {
    //     return IndexedStack(
    //       index: controller.selectedIndex.value,
    //       children: const [
    //         CategoryPage(),
    //         ProfilePage(),
    //         // Container(
    //         //   alignment: Alignment.center,
    //         //   padding: const EdgeInsets.all(15),
    //         //   decoration: const BoxDecoration(
    //         //     color: MyColors.primaryColor,
    //         //     shape: BoxShape.circle,
    //         //   ),
    //         //   child: SvgPicture.asset(
    //         //     svgPath,
    //         //     height: 24,
    //         //     width: 24,
    //         //     color: Colors.white,
    //         //   ),
    //         // ),
    //       ],
    //     );
    //   }),
    //   bottomNavigationBar: CustomBottomNavigationBar(),
    // );
  }
}
