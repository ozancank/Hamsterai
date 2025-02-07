import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/common/common_list_tile.dart';
import 'package:mobile/pages/profile/controller/profile_controller.dart';
import 'package:mobile/pages/profileDetail/controller/profile_detail_controller.dart';
import 'package:mobile/pages/profileDetail/view/profile_detail_page.dart';
import 'package:mobile/pages/settings/view/settings_page.dart';
import 'package:mobile/pages/updatePassword/view/update_password_page.dart';
import 'package:mobile/styles/colors.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final controller = Get.put(ProfileController());
  final profileDetailController = Get.put(ProfileDetailController());
  File? _image;

  @override
  void initState() {
    super.initState();
    loadImage();
  }

  void loadImage() async {
    File? imageFile = await LocaleManager.instance.getImageFromLocal();
    if (imageFile != null) {
      setState(() {
        _image = imageFile;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: const CommonAppbar(title: 'Profil'),
      body: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Expanded(
            flex: 5,
            child: Container(
              padding: EdgeInsets.symmetric(
                horizontal: context.dynamicWidth * 0.04,
              ),
              width: double.infinity,
              color: MyColors.primaryColor,
              child: Column(
                children: [
                  Stack(
                    alignment: Alignment.center,
                    children: [
                      _image == null
                          ? Container(
                              alignment: Alignment.center,
                              padding:
                                  EdgeInsets.all(context.dynamicHeight * 0.04),
                              decoration: BoxDecoration(
                                shape: BoxShape.circle,
                                border:
                                    Border.all(color: Colors.white, width: 8),
                                color: MyColors.grayColor,
                              ),
                              child: SvgPicture.asset(AssetsConstant.person))
                          : CircleAvatar(
                              radius: 60,
                              backgroundImage: FileImage(_image!),
                            ),
                    ],
                  ),
                  SizedBox(height: context.dynamicHeight * 0.01),
                  Text(
                    controller.userModel?.name ?? '',
                    style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                  SizedBox(height: context.dynamicHeight * 0.01),
                  // Container(
                  //   alignment: Alignment.center,
                  //   margin: EdgeInsets.symmetric(
                  //     horizontal: context.dynamicWidth * 0.1,
                  //   ),
                  //   padding: EdgeInsets.symmetric(
                  //       vertical: 5, horizontal: context.dynamicWidth * 0.1),
                  //   decoration: BoxDecoration(
                  //     color: Colors.white,
                  //     borderRadius: BorderRadius.circular(20),
                  //   ),
                  //   child: Text(
                  //     controller.userModel?.email ?? '',
                  //     style: Theme.of(context).textTheme.titleLarge!.copyWith(
                  //           color: MyColors.darkBlue,
                  //           fontWeight: FontWeight.w600,
                  //           overflow: TextOverflow.ellipsis,
                  //         ),
                  //   ),
                  // ),
                ],
              ),
            ),
          ),
          Expanded(
            flex: 9,
            child: Padding(
              padding: EdgeInsets.only(top: context.dynamicHeight * 0.01),
              child: Container(
                alignment: Alignment.center,
                width: double.infinity,
                color: Colors.white,
                child: ListView(
                  children: [
                    CommonListTile(
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        if (controller.userModel?.type == 4) {
                          profileDetailController
                              .getStudentById(controller.userModel!.id);
                        }
                        Get.to(() => const ProfileDetailPage());
                      },
                      iconPath: AssetsConstant.editIcon,
                      text: 'Profil Detay',
                    ),
                    CommonListTile(
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        Get.to(() => const UpdatePasswordPage());
                      },
                      iconPath: AssetsConstant.lockIconWithColor,
                      text: 'Şifre Değiştir',
                    ),
                    CommonListTile(
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        Get.to(() => const SettingsPage());
                      },
                      iconPath: AssetsConstant.notiIcon,
                      text: 'Ayarlar',
                    ),
                    CommonListTile(
                      onTap: () async {
                        HapticFeedback.mediumImpact();
                        await showModalBottomSheet(
                          shape: showModalSheetShape(),
                          backgroundColor: Colors.white,
                          isScrollControlled: true,
                          context: context,
                          builder: (context) {
                            return const CommonBottomSheet();
                          },
                        );
                      },
                      iconPath: AssetsConstant.logoutIcon,
                      text: 'Çıkış Yap',
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }

  RoundedRectangleBorder showModalSheetShape() {
    return const RoundedRectangleBorder(
      borderRadius: BorderRadius.only(
        topRight: Radius.circular(24),
        topLeft: Radius.circular(24),
      ),
    );
  }
}
