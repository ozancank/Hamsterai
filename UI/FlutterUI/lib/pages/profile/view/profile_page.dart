import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/url_storage.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/common/common_list_tile.dart';
import 'package:mobile/pages/profile/controller/profile_controller.dart';
import 'package:mobile/pages/profileDetail/controller/profile_detail_controller.dart';
import 'package:mobile/pages/profileDetail/view/profile_detail_page.dart';
import 'package:mobile/pages/settings/view/settings_page.dart';
import 'package:mobile/pages/updatePassword/view/update_password_page.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/quickalert.dart';

class ProfilePage extends StatefulWidget {
  const ProfilePage({super.key});

  @override
  State<ProfilePage> createState() => _ProfilePageState();
}

class _ProfilePageState extends State<ProfilePage> {
  final controller = Get.put(ProfileController());
  final profileDetailController = Get.put(ProfileDetailController());
  final baseController = Get.put(BaseController());
  String? userPhotoPath;
  var baseUrl;

  Future<String> getImageUrl() async {
    baseUrl = await UrlStorage.getBaseUrl();
    return baseUrl ?? "https://api.hamsterai.com.tr";
  }

  @override
  void initState() {
    super.initState();
    loadUser();
    getImageUrl();
  }

  void loadUser() async {
    await baseController.getUserModelFromCache();
    setState(() {});
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
            flex: 4,
            child: Container(
              padding: EdgeInsets.symmetric(
                horizontal: context.dynamicWidth * 0.04,
              ),
              width: double.infinity,
              decoration: const BoxDecoration(
                color: MyColors.primaryColor,
                borderRadius: BorderRadius.only(
                  bottomLeft: Radius.circular(30),
                  bottomRight: Radius.circular(30),
                ),
              ),
              child: Column(
                children: [
                  controller.userModel != null
                      ? Container(
                          decoration: BoxDecoration(
                            border: Border.all(color: Colors.white),
                            color: Colors.black,
                            shape: BoxShape.circle,
                          ),
                          child: ClipOval(
                            child: SizedBox(
                              width: context.width * 0.34,
                              height: context.height * 0.16,
                              child: CustomImage(
                                isTestResult: true,
                                imageUrl:
                                    '$baseUrl/ProfilePicture/${baseController.userModel?.profileFileName}',
                                headers: ApplicationConstants.XAPIKEY,
                              ),
                            ),
                          ),
                        )
                      : Container(
                          alignment: Alignment.center,
                          padding: EdgeInsets.all(context.dynamicHeight * 0.04),
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            border: Border.all(color: Colors.white, width: 8),
                            color: MyColors.grayColor,
                          ),
                          child: SvgPicture.asset(AssetsConstant.person)),
                  SizedBox(height: context.dynamicHeight * 0.01),
                  Text(
                    baseController.userModel != null
                        ? baseController.userModel!.name!
                        : '',
                    style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                          color: Colors.white,
                          fontWeight: FontWeight.w600,
                        ),
                  ),
                  SizedBox(height: context.dynamicHeight * 0.01),
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
                    // CommonListTile(
                    //   onTap: () {
                    //     HapticFeedback.mediumImpact();
                    //     Get.to(() => const SettingsPage());
                    //   },
                    //   iconPath: AssetsConstant.notiIcon,
                    //   text: 'Ayarlar',
                    // ),
                    CommonListTile(
                      onTap: () async {
                        HapticFeedback.mediumImpact();
                        await showModalBottomSheet(
                          shape: showModalSheetShape(),
                          backgroundColor: Colors.white,
                          isScrollControlled: true,
                          context: context,
                          builder: (context) {
                            return CommonBottomSheet(
                              approveText: 'Bizimle iletişime geçin',
                              approveOnTap: () async {
                                QuickAlert.show(
                                  context: context,
                                  type: QuickAlertType.info,
                                  text:
                                      'Hesabınızı silmek için iletişime geçeceğiz.',
                                  confirmBtnText: 'Tamam',
                                  confirmBtnColor: MyColors.primaryColor,
                                );
                              },
                              smallText: 'Hesabınızı silmek istiyor musunuz?',
                            );
                          },
                        );
                      },
                      iconPath: AssetsConstant.editIcon,
                      icon: Container(
                        padding: const EdgeInsets.all(10),
                        decoration: const BoxDecoration(
                          shape: BoxShape.circle,
                          color: MyColors.primaryColor,
                        ),
                        child: const Icon(
                          Icons.delete_outline_outlined,
                          color: Colors.white,
                        ),
                      ),
                      text: 'Hesap Sil',
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
// Container(
//                     decoration: BoxDecoration(
//                       border: Border.all(color: Colors.white),
//                       color: Colors.black,
//                       shape: BoxShape.circle,
//                     ),
//                     child: ClipOval(
//                       child: SizedBox(
//                         width: context.width * 0.34,
//                         height: context.height * 0.16,
//                         child: CustomImage(
//                           isTestResult: true,
//                           imageUrl:
//                               '${ApplicationConstants.APIBASEURL}/ProfilePicture/${controller.userModel?.profileFileName}',
//                           headers: ApplicationConstants.XAPIKEY,
//                         ),
//                       ),
//                     ),
//                   ),