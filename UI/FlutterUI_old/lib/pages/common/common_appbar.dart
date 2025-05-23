import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/settings/view/settings_page.dart';
import 'package:mobile/styles/colors.dart';

class CommonAppbar extends StatelessWidget implements PreferredSizeWidget {
  const CommonAppbar({super.key, required this.title});
  final String title;

  @override
  Widget build(BuildContext context) {
    return AppBar(
      centerTitle: true,
      backgroundColor: MyColors.primaryColor,
      toolbarHeight: context.dynamicHeight * 0.1,
      leading: GestureDetector(
        onTap: () {
          HapticFeedback.mediumImpact();
          Get.back();
        },
        child: Container(
            padding: const EdgeInsets.all(10),
            child: SvgPicture.asset(AssetsConstant.backbutton)),
      ),
      title: Text(
        title,
        style: Theme.of(context)
            .textTheme
            .headlineSmall!
            .copyWith(color: Colors.white),
      ),
      actions: [
        // InkWell(
        //   onTap: () {
        //     HapticFeedback.mediumImpact();
        //     Get.to(() => const SettingsPage());
        //   },
        //   child: Padding(
        //     padding: const EdgeInsets.all(8.0),
        //     child: SvgPicture.asset(AssetsConstant.settings),
        //   ),
        // ),
      ],
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(60);
}
