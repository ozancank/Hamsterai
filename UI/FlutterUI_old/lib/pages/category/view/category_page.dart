import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/base/base_controller.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/enums/question_role.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/common/home_menu_item2.dart';
import 'package:mobile/pages/home/controller/home_page_controller.dart';
import 'package:mobile/pages/homeworks/view/homework_view.dart';
import 'package:mobile/pages/myQuestions/view/my_questions_page.dart';
import 'package:mobile/pages/myTest/view/choose_lesson.dart';
import 'package:mobile/pages/notifications/view/notification_page.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/pages/scan/view/scan_page.dart';
import 'package:mobile/pages/similarQuestions/view/similar_question_page.dart';
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

  File? _image;

  @override
  void initState() {
    super.initState();
    loadUser();
    loadImage();
  }

  void loadUser() async {
    await homeController.getUserModelFromCache();
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
              // Container(
              //   padding: EdgeInsets.all(context.dynamicHeight * 0.001),
              //   height: context.dynamicHeight * 0.16,
              //   width: double.infinity,
              //   alignment: Alignment.center,
              //   child: Image.asset(AssetsConstant.logo2),
              // ),
              Container(
                alignment: Alignment.center,
                padding: EdgeInsets.all(context.dynamicHeight * 0.04),
                decoration: BoxDecoration(
                  shape: BoxShape.circle,
                  border: Border.all(color: MyColors.primaryColor, width: 5),
                  color: MyColors.grayColor,
                ),
                child: _image == null
                    ? SvgPicture.asset(AssetsConstant.person)
                    : CircleAvatar(
                        radius: 60,
                        backgroundImage: FileImage(_image!),
                      ),
              ),
              const SizedBox(height: 5),
              Align(
                alignment: Alignment.center,
                child: Text(
                  homeController.userModel != null
                      ? 'Hoşgeldin ${homeController.userModel!.name}'
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
              HomeMenuItem2(
                text: 'Benzer Soru Üret',
                trailing: const Icon(
                  Icons.send,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.sendSimilarQuestion,
                iconWidth: 23,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  scanController.setRole(QuestionRole.similarQuestion);
                  Get.to(() {
                    return const ScanPage();
                  });
                },
              ),
              HomeMenuItem2(
                text: 'Sorularım',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.list,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => const MyQuestionsPage());
                },
              ),
              HomeMenuItem2(
                text: 'Benzer Sorularım',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.list,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => const SimilarQuestionPage());
                },
              ),
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
                text: 'Öğretmen Testlerim',
                trailing: SvgPicture.asset(
                  AssetsConstant.rightBack,
                  color: MyColors.primaryColor,
                ),
                iconPath: AssetsConstant.test,
                onTap: () {
                  HapticFeedback.mediumImpact();
                  Get.to(() => const HomeworkView());
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
            ],
          ),
        ),
      ),
    );
  }
}
