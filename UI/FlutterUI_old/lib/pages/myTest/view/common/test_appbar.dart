import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/model/quiz_list_model.dart';
import 'package:mobile/styles/colors.dart';

class TestAppbar extends StatelessWidget implements PreferredSizeWidget {
  final bool isResultPage;
  final int questionIndex;
  final int quizLength;
  final QuizItem? quizItem;
  final bool isResultDetailPage;
  const TestAppbar({
    super.key,
    this.isResultPage = false,
    required this.questionIndex,
    required this.quizLength,
    this.quizItem,
    this.isResultDetailPage = false,
  });

  @override
  Widget build(BuildContext context) {
    final quizController = Get.find<QuizController>();
    return AppBar(
      backgroundColor: Colors.white,
      leading: isResultPage
          ? null
          : Padding(
              padding: const EdgeInsets.all(10),
              child: GestureDetector(
                onTap: () async {
                  if (isResultDetailPage) {
                    Get.back();
                    return;
                  }
                  await showModalBottomSheet(
                    isDismissible: false,
                    enableDrag: false,
                    shape: showModalSheetShape(),
                    backgroundColor: Colors.white,
                    isScrollControlled: true,
                    context: context,
                    builder: (context) {
                      return CommonBottomSheet(
                        headerText: 'Emin misiniz?',
                        smallText: 'Testi tamamlamadan çıkmak üzeresiniz',
                        cancelText: 'İptal',
                        approveText: 'Teste Ara Ver',
                        isScan: true,
                        cancelButtonColor: Colors.white,
                        approveButtonColor: MyColors.primaryColor,
                        cancelTextColor: MyColors.primaryColor,
                        approveOnTap: () async {
                          await quizController.updateQuiz(quizItem!.id,
                              questionIndex: questionIndex);
                          Get.offNamedUntil(
                              '/home', ModalRoute.withName('/home'));
                        },
                      );
                    },
                  );
                },
                child: SvgPicture.asset(
                  AssetsConstant.cancelTest,
                ),
              ),
            ),
      automaticallyImplyLeading: isResultPage ? false : true,
      title: isResultPage
          ? const SizedBox()
          : isResultDetailPage
              ? null
              : Center(
                  child: Container(
                    padding: EdgeInsets.symmetric(
                        horizontal: context.width * 0.05,
                        vertical: context.height * 0.01),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(20),
                      color: Colors.transparent,
                      border: Border.all(
                        color: const Color(0xFFADB4C0),
                      ),
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        SvgPicture.asset(AssetsConstant.alarm),
                        const SizedBox(width: 10),
                        Obx(() => Text(
                              quizController.formattedTime,
                              style: Theme.of(context)
                                  .textTheme
                                  .bodyLarge!
                                  .copyWith(color: MyColors.primaryColor),
                            )),
                      ],
                    ),
                  ),
                ),
      actions: [
        isResultPage
            ? const SizedBox()
            : Row(
                children: [
                  Text(
                    questionIndex.toString(),
                    style: Theme.of(context)
                        .textTheme
                        .bodyLarge!
                        .copyWith(color: const Color(0xFFADB4C0)),
                  ),
                  Text(
                    '/$quizLength',
                    style: Theme.of(context)
                        .textTheme
                        .bodyLarge!
                        .copyWith(color: const Color(0xFFADB4C0)),
                  ),
                ],
              ),
        const SizedBox(width: 10),
      ],
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

  @override
  Size get preferredSize => const Size.fromHeight(60);
}
