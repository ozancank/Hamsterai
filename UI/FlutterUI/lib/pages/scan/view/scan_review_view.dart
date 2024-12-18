import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/pages/scan/model/add_question_picture_post_model.dart';
import 'package:mobile/pages/scan/model/add_question_post_model.dart';
import 'package:mobile/styles/colors.dart';

class ScanReviewView extends StatefulWidget {
  final List<int>? resizedBytes;
  final String base64String;
  final String smallBase64String;
  final File imageFile;
  const ScanReviewView({
    super.key,
    required this.resizedBytes,
    required this.base64String,
    required this.smallBase64String,
    required this.imageFile,
  });

  @override
  State<ScanReviewView> createState() => _ReviewViewState();
}

class _ReviewViewState extends State<ScanReviewView> {
  final scanController = Get.put(ScanController());
  bool questionHasPicture = false;
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar2(title: 'Soru ön izleme'),
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          Column(
            children: [
              const SizedBox(height: 16),
              Image.memory(
                Uint8List.fromList(widget.resizedBytes!),
                width: double.infinity,
                height: context.dynamicHeight * 0.25,
                fit: BoxFit.contain,
              ),
              const SizedBox(height: 16),
              SizedBox(height: context.height * 0.04),
              Text(
                'Soru gönderilmeye hazır',
                style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w600,
                    ),
                textAlign: TextAlign.center,
              ),
              Text(
                'Soruyu gönder cevabı saniyeler içerisinde sende olsun.',
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w400,
                    ),
              ),
              SizedBox(height: context.height * 0.05),
              //! Görselli Soru Butonu
              // scanController.lessonType.value == 1
              //     ? Padding(
              //         padding: const EdgeInsets.symmetric(horizontal: 20),
              //         child: CommonButton(
              //           text:
              //               'Görselli soru gönder\n(Grafik, şekil içeren sorular)',
              //           textColor: Colors.white,
              //           fontSize: 20,
              //           bgcolor: MyColors.primaryColor,
              //           bottomSheet: true,
              //           onTap: () async {
              //             HapticFeedback.mediumImpact();
              //             scanController.scanQuestionWithPicture(
              //                 AddQuestionPicturePostModel(
              //                   lessonId: scanController.lessonId.value,
              //                   questionPictureBase64: widget.base64String,
              //                   questionPictureFileName:
              //                       widget.imageFile.path.split('/').last,
              //                   questionSmallPictureBase64:
              //                       widget.smallBase64String,
              //                   isVisual: true,
              //                 ),
              //                 context);
              //             await Get.delete<ScanController>();
              //             await Get.offAllNamed(
              //               '/home',
              //             );
              //           },
              //         ),
              //       )
              //     : const SizedBox(),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: CommonButton(
                  text: 'Gönder',
                  textColor: Colors.white,
                  bgcolor: MyColors.primaryColor,
                  fontSize: 23,
                  bottomSheet: true,
                  onTap: () async {
                    HapticFeedback.mediumImpact();
                    scanController.scanQuestion(
                        AddQuestionPostModel(
                          lessonId: scanController.lessonId.value,
                          questionPictureBase64: widget.base64String,
                          questionPictureFileName:
                              widget.imageFile.path.split('/').last,
                          questionSmallPictureBase64: widget.smallBase64String,
                        ),
                        context);
                    // Get.offNamedUntil(
                    //   '/home',
                    //   (route) => true,
                    // );
                  },
                ),
              ),
              Padding(
                padding: const EdgeInsets.symmetric(horizontal: 20),
                child: CommonButton(
                  bottomSheet: true,
                  text: 'İptal',
                  fontSize: 23,
                  textColor: Colors.white,
                  bgcolor: const Color(0xFFDD5050),
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    Get.back();
                  },
                ),
              ),
            ],
          ),
        ],
      ),
    );
  }
}
