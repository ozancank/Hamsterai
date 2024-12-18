import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/enums/question_role.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/core/init/cache/url_storage.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/home_menu_item2.dart';
import 'package:mobile/pages/homeworks/view/homework_view.dart';
import 'package:mobile/pages/myTest/view/choose_lesson.dart';
import 'package:mobile/pages/notifications/view/notification_page.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/pages/scan/view/scan_page.dart';
import 'package:mobile/pages/statistics/view/statistics_page.dart';
import 'package:mobile/styles/colors.dart';

class CategoryPage extends StatefulWidget {
  const CategoryPage({super.key});

  @override
  State<CategoryPage> createState() => _CategoryPageState();
}

class _CategoryPageState extends State<CategoryPage> {
  final scanController = Get.put(ScanController());
  final homeController = Get.put(BaseController());
  var baseUrl;
  File? _image;
  @override
  void initState() {
    super.initState();
    loadUser();
    loadImage();
    getImageUrl();
  }

  void loadUser() async {
    await homeController.getUserModelFromCache();
    setState(() {});
  }

  void loadImage() async {
    File? imageFile = await LocaleManager.instance.getImageFromLocal();
    if (imageFile != null) {
      setState(() {
        _image = imageFile;
      });
    }
  }

  Future<String> getImageUrl() async {
    baseUrl = await UrlStorage.getBaseUrl();
    return baseUrl ?? "https://api.hamsterai.com.tr";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Container(
        padding: EdgeInsets.symmetric(horizontal: context.dynamicWidth * 0.05),
        color: Colors.white,
        child: SingleChildScrollView(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              SizedBox(
                height: context.dynamicHeight * 0.03,
              ),
              Padding(
                padding: const EdgeInsets.only(right: 10, top: 20),
                child: Align(
                    alignment: Alignment.topRight,
                    child: GestureDetector(
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        Get.to(() => const NotificationPage());
                      },
                      child: SvgPicture.asset(
                        AssetsConstant.homeNoti,
                        color: MyColors.primaryColor,
                        height: 30,
                      ),
                    )),
              ),

              Container(
                  alignment: Alignment.center,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    border: Border.all(color: MyColors.primaryColor, width: 5),
                    color: MyColors.grayColor,
                  ),
                  child: homeController.userModel == null
                      ? SvgPicture.asset(AssetsConstant.person)
                      : Container(
                          decoration: BoxDecoration(
                            border: Border.all(color: Colors.white),
                            color: Colors.transparent,
                            shape: BoxShape.circle,
                          ),
                          child: ClipOval(
                            child: SizedBox(
                              width: context.width * 0.34,
                              height: context.height * 0.16,
                              child: CustomImage(
                                isTestResult: true,
                                imageUrl:
                                    '$baseUrl/ProfilePicture/${homeController.userModel?.profileFileName}',
                                headers: ApplicationConstants.XAPIKEY,
                              ),
                            ),
                          ),
                        )),
              const SizedBox(height: 5),
              Align(
                alignment: Alignment.center,
                child: Text(
                  homeController.userModel != null
                      ? 'Hoş geldin ${homeController.userModel!.name}'
                      : '',
                  style: Theme.of(context)
                      .textTheme
                      .bodyLarge!
                      .copyWith(color: Colors.grey),
                ),
              ),

              SizedBox(
                height: context.dynamicHeight * 0.03,
              ),
              HomeMenuItem2(
                text: 'Soru Gönder',
                trailing: const Icon(
                  Icons.send,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.sendQuestion,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  scanController.setRole(QuestionRole.question);
                  Get.to(() {
                    return const ScanPage();
                  });
                },
              ),
              // HomeMenuItem2(
              //   text: 'Benzer Soru Üret',
              //   trailing: const Icon(
              //     Icons.send,
              //     color: MyColors.primaryColor,
              //   ),
              //   iconPath: AssetsConstant.sendSimilarQuestion,
              //   onTap: () {
              //     HapticFeedback.mediumImpact();
              //     scanController.setRole(QuestionRole.similarQuestion);
              //     Get.to(() {
              //       return const ScanPage();
              //     });
              //   },
              // ),
              HomeMenuItem2(
                text: 'Sorularım',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.list,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  // Get.to(() => const MyQuestionsPage());
                  Get.toNamed('/my_question_page');
                },
              ),
              // HomeMenuItem2(
              //   text: 'Benzer Sorularım',
              //   trailing: SvgPicture.asset(
              //     AssetsConstant.rightBack,
              //     color: MyColors.primaryColor,
              //   ),
              //   iconPath: AssetsConstant.list,
              //   onTap: () {
              //     HapticFeedback.mediumImpact();
              //     Get.to(() => const SimilarQuestionPage());
              //   },
              // ),
              HomeMenuItem2(
                text: 'Yapay Zeka Testlerim',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.test,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => const ChooseLessonView());
                },
              ),
              HomeMenuItem2(
                text: homeController.userModel != null
                    ? homeController.userModel?.type == 5
                        ? 'Deneme Sınavlarım'
                        : 'Öğretmen Testlerim'
                    : 'Öğretmen Testlerim',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.test,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => HomeworkView(
                        appBarText: homeController.userModel != null
                            ? homeController.userModel?.type == 5
                                ? 'Deneme Sınavlarım'
                                : 'Öğretmen Testlerim'
                            : 'Öğretmen Testlerim',
                      ));
                },
              ),
              HomeMenuItem2(
                text: 'İstatistik',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.chartData,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => const StatisticsPage());
                },
              ),
              // Padding(
              //   padding: EdgeInsets.only(left: context.width * 0.2),
              //   child: HomeMenuItem2(
              //     text:
              //         'Kalan Soru Sayısı: ${homeController.userModel != null ? homeController.userModel!.remainingCredit : ''} Adet',
              //     trailing: SvgPicture.asset(
              //       AssetsConstant.rightBack,
              //       color: MyColors.primaryColor,
              //     ),
              //     iconPath: AssetsConstant.chartData,
              //     remaining: true,
              //     onTap: () {},
              //   ),
              // ),
            ],
          ),
        ),
      ),
    );
  }
}
