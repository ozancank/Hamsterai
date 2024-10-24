import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:intl/intl.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/homeworks/model/homework_model.dart';
import 'package:mobile/styles/colors.dart';
import 'package:syncfusion_flutter_pdfviewer/pdfviewer.dart';

class HomeworkDetailView extends StatefulWidget {
  final HomeWorkItem homeWorkItem;

  const HomeworkDetailView({super.key, required this.homeWorkItem});

  @override
  HomeworkDetailViewState createState() => HomeworkDetailViewState();
}

class HomeworkDetailViewState extends State<HomeworkDetailView> {
  bool isDownloading = false;
  double progress = 0.0;
  void showFullScreenPDF(BuildContext context, String filePath) {
    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => Scaffold(
          backgroundColor: Colors.white,
          body: Stack(
            children: [
              SfPdfViewer.network(
                '${ApplicationConstants.APIBASEURL}/homework/$filePath',
                headers: ApplicationConstants.XAPIKEY,
              ),
              Positioned(
                top: 40,
                right: 10,
                child: GestureDetector(
                  onTap: () {
                    Get.back();
                  },
                  child: Container(
                    padding: const EdgeInsets.all(10),
                    decoration: const BoxDecoration(
                      color: MyColors.primaryColor,
                      shape: BoxShape.circle,
                    ),
                    child: SvgPicture.asset(AssetsConstant.closeFullScreen),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Ödev Detay'),
      backgroundColor: Colors.white,
      body: Container(
        color: Colors.white,
        padding: const EdgeInsets.only(top: 10),
        margin: EdgeInsets.symmetric(
          horizontal: context.dynamicWidth * 0.06,
        ),
        child: Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            color: Colors.white,
            borderRadius: BorderRadius.circular(20),
          ),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Container(
                padding: const EdgeInsets.symmetric(vertical: 10),
                child: Center(
                  child: Column(
                    children: [
                      buildHeader(context, widget.homeWorkItem.homework.id,
                          AssetsConstant.homeworkId),
                      buildHeader(
                          context,
                          widget.homeWorkItem.homework.lessonName,
                          AssetsConstant.homeworkLesson),
                      buildHeader(
                          context,
                          widget.homeWorkItem.homework.teacherName,
                          AssetsConstant.homeworkTeacher),
                      buildHeader(
                          context,
                          DateFormat('dd.MM.yyyy - HH:mm')
                              .format(widget.homeWorkItem.homework.createDate),
                          AssetsConstant.homeworkDate),
                      const SizedBox(height: 10),
                      Container(
                        height: context.height * 0.56,
                        margin: const EdgeInsets.all(8),
                        decoration: BoxDecoration(
                          border: Border.all(color: Colors.grey),
                          borderRadius: BorderRadius.circular(10),
                        ),
                        child: Stack(
                          children: [
                            SfPdfViewer.network(
                              '${ApplicationConstants.APIBASEURL}/homework/${widget.homeWorkItem.homework.filePath}',
                              headers: ApplicationConstants.XAPIKEY,
                            ),
                            Positioned(
                              right: 10,
                              top: 10,
                              child: Column(
                                children: [
                                  GestureDetector(
                                    onTap: () {
                                      showFullScreenPDF(
                                          context,
                                          widget
                                              .homeWorkItem.homework.filePath);
                                    },
                                    child: Container(
                                      padding: const EdgeInsets.all(10),
                                      decoration: const BoxDecoration(
                                        color: MyColors.primaryColor,
                                        shape: BoxShape.circle,
                                      ),
                                      child: SvgPicture.asset(
                                          AssetsConstant.fullPdf),
                                    ),
                                  ),
                                  const SizedBox(height: 5),
                                  GestureDetector(
                                    onTap: () async {
                                      setState(() {
                                        isDownloading = true;
                                      });
                                      await downloadPDF(widget
                                          .homeWorkItem.homework.filePath);
                                      setState(() {
                                        isDownloading = false;
                                      });
                                    },
                                    child: Container(
                                      padding: const EdgeInsets.all(10),
                                      decoration: const BoxDecoration(
                                        color: MyColors.primaryColor,
                                        shape: BoxShape.circle,
                                      ),
                                      child: SvgPicture.asset(
                                          AssetsConstant.pdfDownload),
                                    ),
                                  ),
                                ],
                              ),
                            ),
                          ],
                        ),
                      ),
                      isDownloading
                          ? Padding(
                              padding: const EdgeInsets.symmetric(vertical: 10),
                              child: Text('İndiriliyor... ($progress)'),
                            )
                          : const SizedBox.shrink(),
                    ],
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget buildHeader(BuildContext context, String value, String iconPath) {
    return Row(
      children: [
        SvgPicture.asset(iconPath),
        const SizedBox(width: 10),
        Text(
          value,
          style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                color: const Color(0xFF808080),
                fontWeight: FontWeight.w200,
              ),
        ),
      ],
    );
  }

  Future<void> downloadPDF(String filePath) async {}
}
