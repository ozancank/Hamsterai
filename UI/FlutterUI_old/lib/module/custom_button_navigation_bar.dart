import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/controllers/bottom_nav_controller.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/profile/view/profile_page.dart';
import 'package:mobile/styles/colors.dart';

class CustomBottomNavigationBar extends StatelessWidget {
  final BottomNavController controller = Get.put(BottomNavController());

  CustomBottomNavigationBar({super.key});

  @override
  Widget build(BuildContext context) {
    return BottomAppBar(
      shape: const CircularNotchedRectangle(),
      notchMargin: 8.0,
      height: context.dynamicHeight * 0.13,
      color: Colors.white,
      child: SizedBox(
        child: Padding(
          padding:
              EdgeInsets.symmetric(horizontal: context.dynamicWidth * 0.09),
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              _buildTabItem(svgPath: AssetsConstant.category, index: 0),
              _buildTabItem(svgPath: AssetsConstant.manageAccounts, index: 1),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildTabItem({required String svgPath, required int index}) {
    return GestureDetector(
      onTap: () {
        HapticFeedback.mediumImpact();
        if (index == 1) {
          Get.to(() => const ProfilePage());
        }
        // else {
        //   controller.changeTabIndex(index);
        // }
      },
      child: index == 0
          ? const SizedBox()
          : Container(
              alignment: Alignment.center,
              padding: const EdgeInsets.all(15),
              decoration: const BoxDecoration(
                color: MyColors.primaryColor,
                shape: BoxShape.circle,
              ),
              child: SvgPicture.asset(
                svgPath,
                height: 24,
                width: 24,
                color: Colors.white,
              ),
            ),
    );
  }
}
