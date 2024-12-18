import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/common/dash_divider.dart';
import 'package:mobile/pages/homeworks/controller/homework_controller.dart';
import 'package:mobile/pages/homeworks/view/homework_detail_view.dart';
import 'package:mobile/styles/colors.dart';

class HomeworkView extends StatefulWidget {
  final String appBarText;
  const HomeworkView({super.key, required this.appBarText});

  @override
  State<HomeworkView> createState() => _MyTestViewState();
}

class _MyTestViewState extends State<HomeworkView> {
  var homeworkController = Get.put(HomeworkController());

  Future<void> refreshData() async {
    homeworkController.getHomeworks();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CommonAppbar2(title: widget.appBarText),
      backgroundColor: Colors.white,
      body: Container(
        color: Colors.white,
        padding: const EdgeInsets.only(top: 10),
        margin: EdgeInsets.symmetric(
          horizontal: context.dynamicWidth * 0.06,
        ),
        child: RefreshIndicator(
          color: MyColors.primaryColor,
          onRefresh: refreshData,
          child: Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              // boxShadow: [
              //   BoxShadow(
              //     offset: const Offset(0, 24),
              //     blurRadius: 48,
              //     color: const Color(0xFFB3B8BF).withOpacity(0.3),
              //     spreadRadius: -16,
              //   ),
              // ],
            ),
            child: Obx(() {
              if (homeworkController.loading.value) {
                return const Center(
                  child: CircularProgressIndicator(
                    color: MyColors.primaryColor,
                  ),
                );
              }
              var homeworks = homeworkController.homeWorkList;
              if (homeworks.isEmpty) {
                return Center(
                  child: Text(
                    'Gösterilecek ödev bulunamadı',
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                );
              }

              return ListView.builder(
                itemCount: homeworkController.homeWorkList.length,
                itemBuilder: (context, index) {
                  return GestureDetector(
                    onTap: () {
                      HapticFeedback.mediumImpact();
                      homeworkController.getHomeworks();
                      Get.to(
                        () => HomeworkDetailView(
                          homeWorkItem: homeworkController.homeWorkList[index],
                        ),
                      );
                    },
                    child: Container(
                      padding: const EdgeInsets.only(bottom: 7),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Container(
                                alignment: Alignment.center,
                                padding: const EdgeInsets.all(10),
                                decoration: BoxDecoration(
                                  shape: BoxShape.circle,
                                  color: homeworks[index].status != 3
                                      ? const Color(0xFFD9D9D9)
                                      : MyColors.primaryColor,
                                ),
                                child: Icon(
                                  homeworks[index].status != 3
                                      ? Icons.hourglass_bottom_rounded
                                      : Icons.done,
                                  color: Colors.white,
                                ),
                              ),
                              const SizedBox(width: 15),
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    homeworkController.homeWorkList[index]
                                            .homework.lessonName +
                                        ' Ödevi',
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleMedium!
                                        .copyWith(
                                          color: MyColors.darkBlue,
                                        ),
                                  ),
                                  Text(
                                    homeworks[index].status == 3
                                        ? 'Tamamlandı'
                                        : homeworks[index].status == 2
                                            ? 'Ödeve girildi'
                                            : 'Çözülmesi bekleniyor',
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleSmall!
                                        .copyWith(
                                          color: MyColors.darkBlue,
                                        ),
                                  )
                                ],
                              ),
                            ],
                          ),
                          Padding(
                            padding: const EdgeInsets.all(8.0),
                            child: DashedDivider(
                              dashWidth: 5,
                              dashSpace: 3,
                              color: Colors.grey.withOpacity(0.3),
                            ),
                          ),
                        ],
                      ),
                    ),
                  );
                },
              );
            }),
          ),
        ),
      ),
    );
  }
}
