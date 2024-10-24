import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/view/common/test_appbar.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/styles/colors.dart';

class TestResultDetailView extends StatefulWidget {
  final int index;
  const TestResultDetailView({super.key, required this.index});

  @override
  State<TestResultDetailView> createState() => _TestResultDetailViewState();
}

class _TestResultDetailViewState extends State<TestResultDetailView> {
  final quizController = Get.put(QuizController());
  final scanController = Get.put(ScanController());
  bool isVisible = false;

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      appBar: TestAppbar(
        questionIndex: widget.index + 1,
        quizLength: quizController.quizQuestionList.length,
        isResultDetailPage: true,
      ),
      body: SingleChildScrollView(
        child: Column(
          children: [
            Obx(() {
              return Container(
                margin: const EdgeInsets.symmetric(horizontal: 10),
                child: Align(
                  alignment: Alignment.topCenter,
                  child: CustomImage(
                    isTestResult: true,
                    key: ValueKey(widget.index),
                    imageUrl:
                        '${ApplicationConstants.APIBASEURL}/QuizQuestionPicture/${quizController.quizQuestionList[widget.index].questionPictureFileName}',
                    headers: ApplicationConstants.XAPIKEY,
                  ),
                ),
              );
            }),
            Align(
              alignment: Alignment.bottomCenter,
              child: Container(
                margin: EdgeInsets.symmetric(
                    horizontal: context.dynamicWidth * 0.05),
                child: CommonButton(
                  textColor: Colors.white,
                  bgcolor: MyColors.primaryColor,
                  text: 'Soruyu Çözümünü Göster',
                  onTap: () {
                    setState(() {
                      isVisible = !isVisible;
                    });
                  },
                ),
              ),
            ),
            Obx(() {
              return Visibility(
                visible: isVisible,
                child: Container(
                  margin: const EdgeInsets.symmetric(horizontal: 10),
                  child: Align(
                    alignment: Alignment.topCenter,
                    child: CustomImage(
                      isTestResult: true,
                      key: ValueKey(widget.index),
                      imageUrl:
                          '${ApplicationConstants.APIBASEURL}/QuizAnswerPicture/${quizController.quizQuestionList[widget.index].answerPictureFileName}',
                      headers: ApplicationConstants.XAPIKEY,
                    ),
                  ),
                ),
              );
            }),
          ],
        ),
      ),
      bottomNavigationBar: BottomAppBar(
        color: Colors.transparent,
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceAround,
          children: [
            Obx(() {
              return Row(
                mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                children: List.generate(
                  quizController.quizQuestionList[widget.index].optionCount,
                  (optionIndex) {
                    String label = String.fromCharCode(65 + optionIndex);
                    return _buildSelectableContainer(
                      label,
                      widget.index,
                      quizController.quizModel.value!.id,
                    );
                  },
                ),
              );
            }),
          ],
        ),
      ),
    );
  }

  Widget _buildSelectableContainer(
      String label, int questionNumber, String quizId) {
    String rightOption =
        quizController.quizQuestionList[questionNumber].rightOption;
    String selectedAnswer = quizController.quizModel.value!.answers.values
            .elementAt(questionNumber) ??
        '';

    Color getBackgroundColor() {
      if (label == rightOption) {
        return Colors.green;
      } else if (label == selectedAnswer && selectedAnswer != rightOption) {
        return Colors.red;
      } else {
        return const Color(0xFFF6F7F8);
      }
    }

    Color getTextColor() {
      if (label == rightOption || label == selectedAnswer) {
        return Colors.white;
      } else {
        return Colors.black;
      }
    }

    return Padding(
      padding: EdgeInsets.symmetric(horizontal: context.width * 0.01),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          color: getBackgroundColor(),
          border: Border.all(color: const Color(0xFFD9D9D9)),
        ),
        child: Center(
          child: Text(
            label,
            style: Theme.of(context)
                .textTheme
                .bodyLarge!
                .copyWith(color: getTextColor()),
          ),
        ),
      ),
    );
  }
}
