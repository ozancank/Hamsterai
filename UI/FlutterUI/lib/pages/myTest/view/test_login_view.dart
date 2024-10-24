import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:lottie/lottie.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/model/quiz_list_model.dart';
import 'package:mobile/pages/myTest/view/common/kazanim_container.dart';
import 'package:mobile/pages/myTest/view/test_question_view.dart';
import 'package:mobile/pages/myTest/view/test_result_view.dart';
import 'package:mobile/styles/colors.dart';

class TestLoginView extends StatefulWidget {
  final QuizItem quizItem;
  const TestLoginView({super.key, required this.quizItem});

  @override
  State<TestLoginView> createState() => _TestLoginViewState();
}

class _TestLoginViewState extends State<TestLoginView>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _animation;
  var quizListController = Get.put(QuizController());

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(seconds: 1),
    )..repeat(reverse: true);

    _animation = Tween<double>(begin: 0.0, end: 1.0).animate(_controller);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Test Giriş'),
      backgroundColor: const Color(0xFFF6F7F8),
      // floatingActionButton: Obx(() {
      //   if (quizListController.loading.value) {
      //     return const SizedBox();
      //   }
      //   return FloatingActionButton.extended(
      //     label: Text(
      //       quizListController.quizModel.value?.status == 4
      //           ? 'Sonuca Git'
      //           : 'BAŞLA',
      //       style: Theme.of(context).textTheme.bodyLarge!.copyWith(
      //             color: Colors.white,
      //             fontWeight: FontWeight.bold,
      //           ),
      //     ),
      //     icon: Container(
      //       alignment: Alignment.center,
      //       padding: const EdgeInsets.all(5),
      //       decoration: const BoxDecoration(
      //         color: Colors.white,
      //         shape: BoxShape.circle,
      //       ),
      //       child: SvgPicture.asset(
      //         AssetsConstant.testeBasla,
      //         color: MyColors.primaryColor,
      //       ),
      //     ),
      //     backgroundColor: MyColors.primaryColor,
      //   );
      // }),
      body: Obx(() {
        if (quizListController.loading.value) {
          return const Center(
            child: CircularProgressIndicator(color: MyColors.primaryColor),
          );
        }
        return Stack(
          children: [
            SingleChildScrollView(
              child: Column(
                children: [
                  timer(context),
                  Text(
                    '${widget.quizItem.lessonName} Testi',
                    style: Theme.of(context).textTheme.titleLarge,
                  ),
                  Text(
                    DateTime.now().toIso8601String().toFormattedDateTime2(),
                    style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                          color: const Color(0xFFADB4C0),
                        ),
                  ),
                  Text(
                    'Toplam Soru',
                    style: Theme.of(context)
                        .textTheme
                        .bodyLarge!
                        .copyWith(fontWeight: FontWeight.bold),
                  ),
                  SizedBox(height: context.height * 0.01),
                  DoublePulsingBorderCircle(
                    child: Container(
                      padding: EdgeInsets.all(context.dynamicHeight * 0.035),
                      decoration: const BoxDecoration(
                        shape: BoxShape.circle,
                        color: MyColors.primaryColor,
                      ),
                      child: Center(
                        child: Text(
                          widget.quizItem.questionCount.toString(),
                          style:
                              Theme.of(context).textTheme.titleLarge!.copyWith(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                    fontSize: 30,
                                  ),
                        ),
                      ),
                    ),
                  ),
                  SizedBox(height: context.height * 0.05),
                  const KazanimContainer(
                    border: null,
                    text: 'Kazanımlar',
                    textColor: Color(0xFFADB4C0),
                  ),
                  SizedBox(height: context.height * 0.02),
                  Padding(
                    padding: const EdgeInsets.all(8.0),
                    child: Wrap(
                      spacing: context.width * 0.01,
                      runSpacing: context.height * 0.01,
                      children:
                          widget.quizItem.gainNames.map<Widget>((gainName) {
                        return KazanimContainer(
                          border: Border.all(color: Colors.transparent),
                          text: gainName.length > 50
                              ? '${gainName.substring(0, 10)}...'
                              : gainName,
                          textColor: Colors.white,
                          bgColor: const Color(0xFF94D073),
                          assetName: AssetsConstant.greenCheck,
                          iconBgColor: Colors.white,
                        );
                      }).toList(),
                    ),
                  ),
                  SizedBox(height: context.height * 0.05),
                ],
              ),
            ),
            Align(
              alignment: Alignment.bottomRight,
              child: GestureDetector(
                onTap: () async {
                  if (quizListController.quizModel.value?.status == 4) {
                    Get.to(() => const TestResultView());
                  } else {
                    await quizListController
                        .getQuizQuestions(widget.quizItem.id);
                    await quizListController.quizStarted(widget.quizItem.id);
                    Get.to(() => TestQuestionView(
                          quizItem: widget.quizItem,
                        ));
                  }
                },
                child: Container(
                  padding: EdgeInsets.only(
                      right: context.width * 0.08, top: 5, bottom: 5, left: 5),
                  margin: EdgeInsets.symmetric(
                      vertical: context.height * 0.04,
                      horizontal: context.width * 0.07),
                  decoration: BoxDecoration(
                    borderRadius: BorderRadius.circular(40),
                    color: MyColors.primaryColor,
                  ),
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Container(
                        height: context.height * 0.06,
                        alignment: Alignment.center,
                        padding: const EdgeInsets.all(5),
                        decoration: const BoxDecoration(
                          color: Colors.white,
                          shape: BoxShape.circle,
                        ),
                        child: SvgPicture.asset(
                          AssetsConstant.testeBasla,
                          color: MyColors.primaryColor,
                        ),
                      ),
                      const SizedBox(width: 10),
                      Text(
                        quizListController.quizModel.value?.status == 4
                            ? 'SONUCA GİT'
                            : 'BAŞLA',
                        style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                              color: Colors.white,
                              fontWeight: FontWeight.w900,
                            ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
          ],
        );
      }),
    );
  }

  Container timer(BuildContext context) {
    return Container(
      margin:
          EdgeInsets.symmetric(horizontal: context.width * 0.2, vertical: 10),
      decoration: BoxDecoration(
        color: MyColors.primaryColor,
        borderRadius: BorderRadius.circular(50),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Lottie.asset('assets/json/timer-001.json',
              height: context.height * 0.08),
          Text(
            quizListController.quizModel.value?.status == 4 ||
                    quizListController.quizModel.value?.status == 3
                ? formatTime(quizListController.quizModel.value!.timeSecond)
                : '00:00',
            style: Theme.of(context)
                .textTheme
                .headlineMedium!
                .copyWith(color: Colors.white, fontWeight: FontWeight.w900),
          ),
          const SizedBox(),
          const SizedBox(),
          const SizedBox(),
        ],
      ),
    );
  }

  String formatTime(int timeSecond) {
    int minutes = timeSecond ~/ 60;
    int seconds = timeSecond % 60;
    String formattedMinutes = minutes.toString().padLeft(2, '0');
    String formattedSeconds = seconds.toString().padLeft(2, '0');

    return "$formattedMinutes:$formattedSeconds";
  }
}

class DoublePulsingBorderCircle extends StatefulWidget {
  final Widget child;

  const DoublePulsingBorderCircle({super.key, required this.child});

  @override
  DoublePulsingBorderCircleState createState() =>
      DoublePulsingBorderCircleState();
}

class DoublePulsingBorderCircleState extends State<DoublePulsingBorderCircle>
    with SingleTickerProviderStateMixin {
  late AnimationController _controller;
  late Animation<double> _animation;

  @override
  void initState() {
    super.initState();
    _controller = AnimationController(
      vsync: this,
      duration: const Duration(seconds: 1),
    )..repeat(reverse: true);

    _animation = Tween<double>(begin: 0.0, end: 1.0).animate(_controller);
  }

  @override
  void dispose() {
    _controller.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Stack(
      alignment: Alignment.center,
      children: [
        AnimatedBuilder(
          animation: _animation,
          builder: (context, child) {
            return Container(
              width: 160,
              height: 160,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                border: Border.all(
                  color: MyColors.primaryColor.withOpacity(_animation.value),
                  width: 4.0,
                ),
              ),
            );
          },
        ),
        AnimatedBuilder(
          animation: _animation,
          builder: (context, child) {
            return Container(
              width: 120,
              height: 120,
              decoration: BoxDecoration(
                shape: BoxShape.circle,
                border: Border.all(
                  color: MyColors.primaryColor.withOpacity(_animation.value),
                  width: 2.0,
                ),
              ),
            );
          },
        ),
        widget.child,
      ],
    );
  }
}
