import 'dart:convert';
import 'dart:io';
import 'dart:ui' as ui;
import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:flutter_image_compress/flutter_image_compress.dart';
import 'package:get/get.dart';
import 'package:image_cropper/image_cropper.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mobile/core/enums/question_role.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/scan/model/add_question_post_model.dart';
import 'package:mobile/pages/scan/model/add_question_response_model.dart';
import 'package:mobile/pages/scan/model/lesson_response_model.dart';
import 'package:mobile/pages/scan/model/similar_question_response_model.dart';
import 'package:mobile/pages/scan/service/scan_service.dart';
import 'package:mobile/styles/colors.dart';
import 'package:path/path.dart';
import 'package:path_provider/path_provider.dart';

class ScanController extends GetxController {
  late ScanService _scanService;
  late CameraController cameraController;
  Future<void>? initializeControllerFuture;
  final ImagePicker _picker = ImagePicker();
  var isCameraInitialized = false.obs;
  var hasCameraError = false.obs;

  var lessonId = 0.obs;
  RxList<Lessons> lessons = <Lessons>[].obs;
  RxBool isLoading = true.obs;
  RxBool shouldShowOverlay = false.obs;
  var selectedRole = QuestionRole.question.obs;
  var layoutMode = false.obs;
  var isFlashOn = true.obs;
  File? firstImageFile;

  File? secondImage;
  @override
  void onInit() {
    _scanService = ScanService(NetworkManager.instance.dio);
    // getLessons();
    super.onInit();
  }

  @override
  void dispose() {
    cameraController.setFlashMode(FlashMode.off);
    cameraController.dispose();
    super.dispose();
  }

  void setRole(QuestionRole role) {
    selectedRole.value = role;
  }

  Future<void> initializeCamera() async {
    try {
      final cameras = await availableCameras();
      cameraController = CameraController(
        cameras.first,
        ResolutionPreset.high,
      );

      await cameraController.initialize();
      cameraController.setFlashMode(FlashMode.torch);
      isCameraInitialized.value = true;
    } catch (e) {
      hasCameraError.value = true;
    }
  }

  Future<void> toggleFlash() async {
    if (isFlashOn.value) {
      await cameraController.setFlashMode(FlashMode.off);
      isFlashOn.value = false;
    } else {
      await cameraController.setFlashMode(FlashMode.torch);
      isFlashOn.value = true;
    }
  }

  // Future<void> autoFlashInLowLight() async {
  //   await cameraController.setFlashMode(FlashMode.auto);
  // }

  void onLayerIconTapped(context) async {
    final firstImage = await takeAndCropPicture(true, context);
    if (firstImage != null) {
      cameraController.setFlashMode(FlashMode.off);
      firstImageFile = firstImage;
      //  await captureSecondImage(context, firstImage);
    }
  }

  Future<void> captureSecondImage(context, File firstImage) async {
    final secondImage = await takeAndCropPicture(false, context);
    if (secondImage != null) {
      cameraController.setFlashMode(FlashMode.off);
      await mergeImagesAndSendToAPI(firstImage, secondImage, context);
    }
  }

  Future<File?> takeAndCropPicture(bool isFirst, BuildContext context) async {
    try {
      final image = await cameraController.takePicture();
      await Future.delayed(const Duration(milliseconds: 500));
      cameraController.setFlashMode(FlashMode.off);
      final croppedImage = await _cropImageLayout(image.path, context);

      if (croppedImage != null) {
        return croppedImage;
      }
    } catch (e) {
      print(e);
    }
    return null;
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

  Future<void> mergeImagesAndSendToAPI(
      File firstImage, File secondImage, context) async {
    try {
      final mergedImageBytes = await mergeImages(firstImage, secondImage);
      final base64Image = base64Encode(mergedImageBytes);

      await showModalBottomSheet(
        shape: showModalSheetShape(),
        backgroundColor: Colors.white,
        isScrollControlled: true,
        context: context,
        builder: (context) {
          return CommonBottomSheet(
            headerText: 'Soru gönderilmeye hazır',
            smallText: 'Soruyu gönder cevabı saniyeler içerisinde sende olsun.',
            cancelText: 'İptal',
            approveText: 'Gönder',
            isScan: true,
            cancelButtonColor: const Color(0xFFDD5050),
            approveOnTap: () async {
              HapticFeedback.mediumImpact();
              cameraController.setFlashMode(FlashMode.off);
              scanQuestion(
                  AddQuestionPostModel(
                    lessonId: lessonId.value,
                    questionPictureBase64: base64Image,
                    questionPictureFileName: "merged_image.png",
                  ),
                  context);
              Get.back();
            },
          );
        },
      );

      print("Resim başarıyla gönderildi");
    } catch (e) {
      print("Hata oluştu: $e");
    }
  }

  Future<void> takePicture(context) async {
    try {
      await initializeControllerFuture;
      final image = await cameraController.takePicture();
      _cropImage(image.path, context);
    } catch (e) {
      print(e);
    }
  }

  Future<void> pickImageFromGallery(context) async {
    final pickedFile = await _picker.pickImage(source: ImageSource.gallery);
    if (pickedFile != null) {
      _cropImage(pickedFile.path, context);
    }
  }

  Future<void> _cropImage(String imagePath, BuildContext context) async {
    final croppedImage = await ImageCropper().cropImage(
      sourcePath: imagePath,
      // aspectRatioPresets: [
      //   CropAspectRatioPreset.square,
      //   CropAspectRatioPreset.ratio3x2,
      //   CropAspectRatioPreset.ratio4x3,
      //   CropAspectRatioPreset.ratio16x9
      // ],
      androidUiSettings: const AndroidUiSettings(
        toolbarTitle: 'Kırp',
        toolbarColor: MyColors.primaryColor,
        toolbarWidgetColor: Colors.white,
        initAspectRatio: CropAspectRatioPreset.original,
        cropFrameColor: MyColors.primaryColor,
        backgroundColor: Colors.white,
        lockAspectRatio: false,
      ),
      iosUiSettings: const IOSUiSettings(
        minimumAspectRatio: 1.0,
        title: 'Kırp',
      ),
    );

    if (croppedImage != null) {
      final directory = await getApplicationDocumentsDirectory();
      final fileName = basename(croppedImage.path);
      final newPath = '${directory.path}/$fileName';

      final croppedImageFile = File(croppedImage.path);
      final newImageFile = await croppedImageFile.copy(newPath);
      compressImageToBase64(newImageFile, context);
    }
  }

  Future<File?> _cropImageLayout(String imagePath, BuildContext context) async {
    cameraController.setFlashMode(FlashMode.off);
    final croppedImage = await ImageCropper().cropImage(
      sourcePath: imagePath,
      aspectRatioPresets: [
        CropAspectRatioPreset.square,
        CropAspectRatioPreset.ratio3x2,
        CropAspectRatioPreset.ratio4x3,
        CropAspectRatioPreset.ratio16x9
      ],
      androidUiSettings: const AndroidUiSettings(
        toolbarTitle: 'Kırp',
        toolbarColor: Colors.deepOrange,
        toolbarWidgetColor: Colors.white,
        initAspectRatio: CropAspectRatioPreset.original,
        lockAspectRatio: false,
      ),
      iosUiSettings: const IOSUiSettings(
        minimumAspectRatio: 1.0,
        title: 'Kırp',
      ),
    );

    if (croppedImage != null) {
      final directory = await getApplicationDocumentsDirectory();
      final fileName = basename(croppedImage.path);
      final newPath = '${directory.path}/$fileName';
      final croppedImageFile = File(croppedImage.path);
      return await croppedImageFile.copy(newPath);
    } else {
      return null;
    }
  }

  Future<String?> compressImageToBase64(File imageFile, context) async {
    final result = await FlutterImageCompress.compressAndGetFile(
      imageFile.absolute.path,
      '${imageFile.path}_compressed.jpg',
      quality: 80,
    );
    if (result != null) {
      final bytes = await result.readAsBytes();
      final base64String = base64Encode(bytes);
      await showModalBottomSheet(
        shape: showModalSheetShape(),
        backgroundColor: Colors.white,
        isScrollControlled: true,
        context: context,
        builder: (context) {
          return CommonBottomSheet(
            headerText: 'Soru gönderilmeye hazır',
            smallText: 'Soruyu gönder cevabı saniyeler içerisinde sende olsun.',
            cancelText: 'İptal',
            approveText: 'Gönder',
            isScan: true,
            cancelButtonColor: const Color(0xFFDD5050),
            approveOnTap: () async {
              HapticFeedback.mediumImpact();

              scanQuestion(
                  AddQuestionPostModel(
                      lessonId: lessonId.value,
                      questionPictureBase64: base64String,
                      questionPictureFileName: imageFile.path.split('/').last),
                  context);
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
    isLoading.value = true;
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
