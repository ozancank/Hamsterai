import 'package:auto_size_text/auto_size_text.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/view/my_test_view.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/styles/colors.dart';

class ChooseLessonView extends StatefulWidget {
  const ChooseLessonView({super.key});

  @override
  State<ChooseLessonView> createState() => _ChooseLessonViewState();
}

class _ChooseLessonViewState extends State<ChooseLessonView> {
  final scanController = Get.put(ScanController());
  final quizController = Get.put(QuizController());

  @override
  void initState() {
    scanController.getLessons(() {
      WidgetsBinding.instance.addPostFrameCallback((_) {});
    }, context);
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar2(title: 'Ders Seç'),
      backgroundColor: Colors.white,
      body: SingleChildScrollView(
        child: Column(
          children: [
            SizedBox(height: context.height * 0.05),
            Obx(() {
              if (scanController.isLoading.value) {
                return const Center(
                  child: CircularProgressIndicator(
                    color: MyColors.primaryColor,
                  ),
                );
              }

              return ListView.builder(
                shrinkWrap: true,
                physics: const NeverScrollableScrollPhysics(),
                padding: const EdgeInsets.only(bottom: 20),
                itemCount: scanController.lessons.length,
                itemBuilder: (context, index) {
                  return Container(
                    alignment: Alignment.centerLeft,
                    padding: const EdgeInsets.all(5),
                    child: Column(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        GestureDetector(
                          onTap: () {
                            quizController.lessonId.value =
                                scanController.lessons[index].id;
                            quizController.getQuizList();
                            Get.to(() => const MyTestView());
                          },
                          child: Container(
                            padding: EdgeInsets.only(
                                left: 5, right: context.width * 0.09),
                            margin: EdgeInsets.symmetric(
                                horizontal: context.width * 0.05),
                            decoration: BoxDecoration(
                              color: MyColors.primaryColor,
                              borderRadius: BorderRadius.circular(40),
                            ),
                            child: Row(
                              mainAxisAlignment: MainAxisAlignment.start,
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Container(
                                  padding: const EdgeInsets.all(15),
                                  decoration: const BoxDecoration(
                                    color: Colors.white,
                                    shape: BoxShape.circle,
                                  ),
                                  child: Center(
                                    child: SvgPicture.asset(
                                      AssetsConstant.rightBack,
                                      color: MyColors.primaryColor,
                                      height: 20,
                                    ),
                                    // child: Icon(
                                    //   Icons.arrow_forward_ios_sharp,
                                    //   color: MyColors.primaryColor,
                                    // ),
                                  ),
                                ),
                                const SizedBox(width: 10),
                                Flexible(
                                  child: Text(
                                    scanController.lessons[index].name,
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleLarge!
                                        .copyWith(
                                          color: Colors.white,
                                          fontWeight: FontWeight.w600,
                                        ),
                                  ),
                                ),
                              ],
                            ),
                          ),
                        ),
                      ],
                    ),
                  );
                },
              );
            }),
          ],
        ),
      ),
    );
  }
}
