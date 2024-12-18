import 'dart:convert';
import 'dart:io';
import 'dart:ui' as ui;
import 'package:camera/camera.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:get/get.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mailer/mailer.dart';
import 'package:mailer/smtp_server/gmail.dart';
import 'package:mobile/core/enums/question_role.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/scan/model/add_question_picture_post_model.dart';
import 'package:mobile/pages/scan/model/add_question_post_model.dart';
import 'package:mobile/pages/scan/model/add_question_response_model.dart';
import 'package:mobile/pages/scan/model/lesson_response_model.dart';
import 'package:mobile/pages/scan/model/similar_question_response_model.dart';
import 'package:mobile/pages/scan/service/scan_service.dart';
import 'package:mobile/pages/scan/view/crop_view.dart';
import 'package:mobile/styles/colors.dart';
import 'package:path_provider/path_provider.dart';
import 'package:image/image.dart' as img;

class ScanController extends GetxController {
  late ScanService _scanService;
  late CameraController? cameraController;
  Future<void>? initializeControllerFuture;
  final ImagePicker _picker = ImagePicker();
  var isCameraInitialized = false.obs;
  var hasCameraError = false.obs;
  var lessonId = 0.obs;
  var lessonType = 1.obs;
  RxList<Lessons> lessons = <Lessons>[].obs;
  RxBool isLoading = true.obs;
  RxBool shouldShowOverlay = false.obs;
  var selectedRole = QuestionRole.question.obs;
  var layoutMode = false.obs;
  var pickSecondImage = false.obs;
  var isFlashOn = false.obs;
  File? firstImageFile;
  File? secondImageFile;
  RxBool questionHasPicture = false.obs;
  @override
  void onInit() {
    _scanService = ScanService(NetworkManager.instance.dio);
    super.onInit();
  }

  @override
  void onClose() {
    cameraController?.setFlashMode(FlashMode.off);
    cameraController?.dispose();
    super.onClose();
  }

  void setRole(QuestionRole role) {
    selectedRole.value = role;
  }

  Future<void> initializeCamera() async {
    isCameraInitialized.value = false;
    isLoading.value = true;
    try {
      final cameras = await availableCameras();
      cameraController = CameraController(
        cameras.first,
        ResolutionPreset.high,
      );
      await cameraController?.initialize();
      cameraController?.setFlashMode(FlashMode.off);
      isCameraInitialized.value = true;
      isLoading.value = false;
    } catch (e) {
      hasCameraError.value = true;
      cameraController = null;
      isLoading.value = false;
      await sendErrorEmail(e.toString());
      rethrow;
    }
  }

  Future<void> sendErrorEmail(String errorMessage) async {
    const String username = 'sedusstest@gmail.com';
    const String password = 'qnhcnfawqwyzzwgm';
    final smtpServer = gmail(username, password);

    final message = Message()
      ..from = const Address(username, 'SEDUSS Camera Error')
      ..recipients.add('942alicankesen@gmail.com')
      ..subject = 'Camera Error Notification'
      ..text = 'An error occurred in initializeCamera: $errorMessage';

    try {
      await send(message, smtpServer);
      print('Error email sent successfully.');
    } catch (e) {
      print('Failed to send error email: $e');
    }
  }

  Future<void> toggleFlash() async {
    if (isFlashOn.value) {
      await cameraController?.setFlashMode(FlashMode.off);
      isFlashOn.value = false;
    } else {
      await cameraController?.setFlashMode(FlashMode.torch);
      isFlashOn.value = true;
    }
  }

  void onLayerIconTapped(BuildContext context, bool isFirst) async {
    await initializeControllerFuture;
    final image = await cameraController?.takePicture();
    final imageBytes = await _imageToUint8List(image!.path);
    cameraController?.setFlashMode(FlashMode.off);

    Get.to(() => CropView(
          image: imageBytes,
          isSecondToGo: isFirst ? true : false,
          doublePhoto: true,
        ));
  }

  Future<Uint8List> mergeImages(
      File firstImageFile, File secondImageFile) async {
    final firstImageBytes = await firstImageFile.readAsBytes();
    final firstImage = await decodeImageFromList(firstImageBytes);
    final secondImageBytes = await secondImageFile.readAsBytes();
    final secondImage = await decodeImageFromList(secondImageBytes);
    final newWidth = firstImage.width > secondImage.width
        ? firstImage.width
        : secondImage.width;
    final newHeight = firstImage.height + secondImage.height;
    final recorder = ui.PictureRecorder();
    final canvas = Canvas(
        recorder,
        Rect.fromPoints(const Offset(0, 0),
            Offset(newWidth.toDouble(), newHeight.toDouble())));
    canvas.drawImage(firstImage, const Offset(0, 0), Paint());
    canvas.drawImage(
        secondImage, Offset(0, firstImage.height.toDouble()), Paint());
    final picture = recorder.endRecording();
    final img = await picture.toImage(newWidth, newHeight);
    final byteData = await img.toByteData(format: ui.ImageByteFormat.png);
    final imageBytes = byteData!.buffer.asUint8List();

    return imageBytes;
  }

  Future<File> convertUint8ListToFile(Uint8List uint8List) async {
    final tempDir = await getTemporaryDirectory();
    final file = File('${tempDir.path}/temp_image.png');
    await file.writeAsBytes(uint8List);
    return file;
  }

  Future<void> mergeImagesAndSendToAPI(context) async {
    try {
      final mergedImageBytes =
          await mergeImages(firstImageFile!, secondImageFile!);
      compressImageToBase64(
          await convertUint8ListToFile(mergedImageBytes), context);
    } catch (e) {
      if (kDebugMode) {
        print("Hata oluştu: $e");
      }
    }
  }

  Future<void> takePicture(context) async {
    try {
      await initializeControllerFuture;
      final image = await cameraController?.takePicture();
      final imageBytes = await _imageToUint8List(image!.path);
      Get.to(() => CropView(
            image: imageBytes,
            doublePhoto: false,
          ));
    } catch (e) {
      if (kDebugMode) {
        print(e);
      }
    }
  }

  Future<Uint8List> _imageToUint8List(String imagePath) async {
    final file = File(imagePath);
    return file.readAsBytes();
  }

  Future<void> pickImageFromGallery(context) async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      final imageBytes = await _imageToUint8List(pickedFile.path);
      Get.to(() => CropView(
            image: imageBytes,
            doublePhoto: false,
          ));
    }
  }

  Future<void> pickDoubleImageFromGallery(context) async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      final imageBytes = await _imageToUint8List(pickedFile.path);
      Get.to(() => CropView(
            image: imageBytes,
            doublePhoto: true,
            isSecondToGo: pickSecondImage.value ? false : true,
          ));
    }
  }

  Future<String?> compressImageToBase64(File imageFile, context) async {
    final result1 = await FlutterImageCompress.compressAndGetFile(
      imageFile.absolute.path,
      '${imageFile.path}_compressed.jpg',
      quality: 80,
    );
    final bytes = await imageFile.readAsBytes();
    final originalImage = img.decodeImage(bytes);

    if (originalImage == null) {
      return null;
    }
    final int originalWidth = originalImage.width;
    final int originalHeight = originalImage.height;
    // Uzun kenarı ve küçültme oranını hesaplayın
    final int longestSide =
        originalWidth > originalHeight ? originalWidth : originalHeight;
    final double scaleRatio = 512 / longestSide;

    // Yeni genişlik ve yükseklik hesaplayın
    final int newWidth = (originalWidth * scaleRatio).round();
    final int newHeight = (originalHeight * scaleRatio).round();

    // Resmi yeniden boyutlandırın
    final resizedImage =
        img.copyResize(originalImage, width: newWidth, height: newHeight);

    // Yeni resmi sıkıştırın
    final resizedBytes = img.encodeJpg(resizedImage);

    if (result1 != null) {
      final bytes = await result1.readAsBytes();

      final base64String = base64Encode(bytes);
      final smallBase64String = base64Encode(resizedBytes);
      Get.back();
      EasyLoading.dismiss();

      Future.delayed(const Duration(milliseconds: 100));

      // Get.to(
      //   () => ScanReviewView(
      //     resizedBytes: resizedBytes,
      //     base64String: base64String,
      //     smallBase64String: smallBase64String,
      //     imageFile: imageFile,
      //   ),
      // );
      await showModalBottomSheet(
        shape: showModalSheetShape(),
        backgroundColor: Colors.white,
        isDismissible: false,
        isScrollControlled: true,
        context: Get.context!,
        builder: (context) {
          return CommonBottomSheet(
            headerText: 'Soru gönderilmeye hazır',
            smallText: 'Soruyu gönder cevabı saniyeler içerisinde sende olsun.',
            cancelText: 'İptal',
            approveText: 'Gönder',
            isScan: true,
            resizedBytes: resizedBytes,
            cancelButtonColor: const Color(0xFFDD5050),
            approveOnTap: () async {
              HapticFeedback.mediumImpact();
              scanQuestion(
                  AddQuestionPostModel(
                    lessonId: lessonId.value,
                    questionPictureBase64: base64String,
                    questionPictureFileName: imageFile.path.split('/').last,
                    questionSmallPictureBase64: smallBase64String,
                  ),
                  context);
              // Get.offAllNamed('/home');
              Get.back();
            },
          );
        },
      );

      return base64String;
    } else {
      return null;
    }
  }

  RoundedRectangleBorder showModalSheetShape() {
    return const RoundedRectangleBorder(
      borderRadius: BorderRadius.only(
        topRight: Radius.circular(24),
        topLeft: Radius.circular(24),
      ),
    );
  }

  Future<void> scanQuestion(AddQuestionPostModel model, context) async {
    try {
      EasyLoading.show(
        status: 'Soru Gönderiliyor',
        maskType: EasyLoadingMaskType.black,
      );
      dynamic scanModel;
      switch (selectedRole.value) {
        case QuestionRole.question:
          scanModel = await _scanService.scanQuestion(model.toJson());
          break;
        case QuestionRole.similarQuestion:
          scanModel = await _scanService.scanSimilarQuestion(model.toJson());
          break;
      }
      if (scanModel is AddQuestionResponseModel ||
          scanModel is SimilarQuestionResponseModel) {
        _handleSuccess();
      } else if (scanModel is ErrorModel) {
        EasyLoading.dismiss();
        Get.snackbar(
          'Hata',
          scanModel.message,
          backgroundColor: MyColors.redColor,
          colorText: Colors.white,
          snackPosition: SnackPosition.TOP,
          duration: const Duration(seconds: 3),
        );
        Get.offAllNamed('/home');
      }
    } catch (e) {
      EasyLoading.dismiss();
      Get.snackbar(
        'Hata',
        e.toString(),
        backgroundColor: MyColors.redColor,
        colorText: Colors.white,
        snackPosition: SnackPosition.TOP,
        duration: const Duration(seconds: 3),
      );
      Get.offAllNamed('/home');
    }
  }

  Future<void> scanQuestionWithPicture(
      AddQuestionPicturePostModel model, context) async {
    try {
      EasyLoading.show(
        status: 'Soru Gönderiliyor',
        maskType: EasyLoadingMaskType.black,
      );
      dynamic scanModel;
      switch (selectedRole.value) {
        case QuestionRole.question:
          scanModel = await _scanService.scanQuestion(model.toJson());
          break;
        case QuestionRole.similarQuestion:
          scanModel = await _scanService.scanSimilarQuestion(model.toJson());
          break;
      }
      if (scanModel is AddQuestionResponseModel ||
          scanModel is SimilarQuestionResponseModel) {
        _handleSuccess();
      } else if (scanModel is ErrorModel) {
        EasyLoading.dismiss();
        Get.snackbar(
          'Hata',
          scanModel.message,
          backgroundColor: MyColors.redColor,
          colorText: Colors.white,
          snackPosition: SnackPosition.TOP,
          duration: const Duration(seconds: 3),
        );
        Get.offAllNamed('/home');
      }
    } catch (e) {
      EasyLoading.dismiss();
      Get.snackbar(
        'Hata',
        e.toString(),
        backgroundColor: MyColors.redColor,
        colorText: Colors.white,
        snackPosition: SnackPosition.TOP,
        duration: const Duration(seconds: 3),
      );
      Get.offAllNamed('/home');
    }
  }

  void _handleSuccess() {
    EasyLoading.dismiss();
    Future.delayed(Duration.zero, () {
      Get.find<ScanController>().shouldShowOverlay.value = true;
    });
  }

  void _handleError(ErrorModel erroModel, BuildContext context) {}

  Future<void> getLessons(Function onLessonsLoaded, context) async {
    WidgetsBinding.instance.addPostFrameCallback((_) {
      isLoading.value = true;
    });
    var lessonModel = await _scanService.getLessons();
    if (lessonModel is LessonResponseModel) {
      lessons.value = lessonModel.items;
    } else if (lessonModel is ErrorModel) {
      _handleError(lessonModel, context);
    }
    isLoading.value = false;
    onLessonsLoaded();
  }
}
