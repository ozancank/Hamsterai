import 'dart:convert';
import 'dart:io';

import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:image_picker/image_picker.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/local_manager.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/profile/controller/profile_controller.dart';
import 'package:mobile/pages/profileDetail/controller/profile_detail_controller.dart';
import 'package:mobile/pages/profileDetail/model/update_user_post_model.dart';
import 'package:mobile/pages/profileDetail/view/common/profile_text_field.dart';
import 'package:mobile/styles/colors.dart';
import 'package:quickalert/quickalert.dart';

class ProfileDetailPage extends StatefulWidget {
  const ProfileDetailPage({super.key});

  @override
  State<ProfileDetailPage> createState() => _ProfileDetailPageState();
}

class _ProfileDetailPageState extends State<ProfileDetailPage> {
  final formKey = GlobalKey<FormState>();
  final profileController = Get.put(ProfileController());
  final updateUserController = Get.put(ProfileDetailController());
  final nameController = TextEditingController();
  final surnameController = TextEditingController();
  final emailController = TextEditingController();
  final phoneController = TextEditingController();
  final genderController = TextEditingController();
  File? _image;
  final ImagePicker _picker = ImagePicker();
  String? base64Image;
  String? imagePath;

  void loadImage() async {
    File? imageFile = await LocaleManager.instance.getImageFromLocal();
    if (imageFile != null) {
      setState(() {
        _image = imageFile;
      });
    }
  }

  Future<void> pickImage() async {
    final ImageSource? source = await showDialog<ImageSource>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('Fotoğraf Kaynağını Seçin'),
        actions: [
          TextButton(
            onPressed: () => Navigator.of(context).pop(ImageSource.camera),
            child: const Text('Kamera'),
          ),
          TextButton(
            onPressed: () => Navigator.of(context).pop(ImageSource.gallery),
            child: const Text('Galeri'),
          ),
        ],
      ),
    );

    if (source != null) {
      final XFile? image = await _picker.pickImage(source: source);
      if (image != null) {
        imagePath = image.path;
        _image = File(image.path);
        List<int> imageBytes = await _image!.readAsBytes();
        String base64String = base64Encode(imageBytes);
        setState(() {
          base64Image = base64String;
        });
      }
    }
  }

  @override
  void initState() {
    nameController.text = profileController.userModel!.name!;
    surnameController.text = profileController.userModel!.surname!;
    emailController.text = profileController.userModel!.email!;
    phoneController.text = profileController.userModel!.phone!;
    loadImage();
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Profil Detay'),
      body: Container(
        color: Colors.white,
        height: context.dynamicHeight,
        child: SingleChildScrollView(
          child: Form(
            key: formKey,
            child: Column(
              children: [
                SizedBox(height: context.dynamicHeight * 0.02),
                Stack(
                  alignment: Alignment.center,
                  children: [
                    _image == null
                        ? Container(
                            alignment: Alignment.center,
                            padding:
                                EdgeInsets.all(context.dynamicHeight * 0.04),
                            decoration: BoxDecoration(
                              shape: BoxShape.circle,
                              border: Border.all(color: Colors.white, width: 8),
                              color: MyColors.grayColor,
                            ),
                            child: SvgPicture.asset(AssetsConstant.person))
                        : CircleAvatar(
                            radius: 60,
                            backgroundImage: FileImage(_image!),
                          ),
                    Positioned(
                      top: context.dynamicHeight * 0.09,
                      child: GestureDetector(
                        onTap: () async {
                          pickImage();
                        },
                        child: Container(
                          alignment: Alignment.center,
                          padding:
                              EdgeInsets.all(context.dynamicHeight * 0.015),
                          decoration: BoxDecoration(
                            color: MyColors.primaryColor,
                            shape: BoxShape.circle,
                            border: Border.all(color: Colors.white, width: 6),
                          ),
                          child: SvgPicture.asset(AssetsConstant.edit),
                        ),
                      ),
                    ),
                  ],
                ),
                ProfileTextField(
                  controller: nameController,
                  hintText: 'Ad',
                  isGrey: true,
                  validator: (value) {
                    if (value!.isEmpty) {
                      return '';
                    }
                    return null;
                  },
                ),
                ProfileTextField(
                  controller: surnameController,
                  hintText: 'Soyad',
                  isGrey: true,
                  validator: (value) {
                    if (value!.isEmpty) {
                      return '';
                    }
                    return null;
                  },
                ),
                ProfileTextField(
                  controller: emailController,
                  hintText: 'E-mail',
                  keyboardType: TextInputType.emailAddress,
                  isGrey: true,
                  validator: (value) {
                    if (value!.isEmpty && !value.isEmail) {
                      return '';
                    }
                    return null;
                  },
                ),
                ProfileTextField(
                  controller: phoneController,
                  hintText: 'Telefon',
                  keyboardType: TextInputType.number,
                  isGrey: true,
                  validator: (value) {
                    if (value!.isEmpty && !value.isPhoneNumber) {
                      return '';
                    }
                    return null;
                  },
                ),
                ProfileTextField(
                  controller: genderController,
                  hintText: 'Okul',
                  isGrey: true,
                  validator: (value) {},
                ),
                ProfileTextField(
                  controller: genderController,
                  hintText: 'Sınıf',
                  isGrey: true,
                  validator: (value) {},
                ),
                Visibility(
                  visible: false,
                  child: Padding(
                    padding: EdgeInsets.symmetric(
                      horizontal: context.dynamicWidth * 0.05,
                    ),
                    child: CommonButton(
                      textColor: Colors.white,
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        if (!formKey.currentState!.validate()) {
                          QuickAlert.show(
                            context: context,
                            type: QuickAlertType.error,
                            text: 'Lütfen tüm alanları doldurunuz.',
                            confirmBtnText: 'Tamam',
                            confirmBtnColor: MyColors.redColor,
                            confirmBtnTextStyle: Theme.of(context)
                                .textTheme
                                .bodyMedium!
                                .copyWith(color: Colors.white),
                            title: 'Hata',
                          );
                          return;
                        }
                        updateUserController.updateUser(
                            UpdateUserPostModel(
                              id: profileController.userModel!.id,
                              userName: profileController.userModel!.userName!,
                              name: nameController.text,
                              surname: surnameController.text,
                              phone: phoneController.text,
                              profileUrl: '',
                              email: emailController.text,
                              type: profileController.userModel!.type!,
                              connectionId: 0,
                              schoolId: 1,
                              profilePictureBase64: base64Image ?? '',
                              profilePictureFileName: imagePath ?? '',
                            ),
                            context);
                      },
                      bgcolor: MyColors.primaryColor,
                      text: 'Güncelle',
                    ),
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }

  // void _showImageSourceActionSheet(BuildContext context) {
  //   showModalBottomSheet(
  //     context: context,
  //     builder: (BuildContext context) {
  //       return SafeArea(
  //         child: Wrap(
  //           children: <Widget>[
  //             ListTile(
  //               leading: const Icon(Icons.photo_library),
  //               title: const Text('Galeriden Seç'),
  //               onTap: () {
  //                 Get.back();
  //                 pickImage(ImageSource.gallery);
  //               },
  //             ),
  //             ListTile(
  //               leading: const Icon(Icons.photo_camera),
  //               title: const Text('Kamerayı Kullan'),
  //               onTap: () {
  //                 Get.back();
  //                 pickImage(ImageSource.camera);
  //               },
  //             ),
  //           ],
  //         ),
  //       );
  //     },
  //   );
  // }

  Stack editProfilePage(BuildContext context) {
    return Stack(
      alignment: Alignment.center,
      children: [
        Container(
          alignment: Alignment.center,
          padding: EdgeInsets.all(context.dynamicHeight * 0.04),
          decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(color: Colors.white, width: 8),
              color: MyColors.grayColor,
              boxShadow: [
                BoxShadow(
                  offset: const Offset(0, 10),
                  blurRadius: 40,
                  color: const Color(0xFF888888).withOpacity(0.4),
                ),
              ]),
          child: SvgPicture.asset(AssetsConstant.person),
        ),
        Positioned(
          top: context.dynamicHeight * 0.1,
          child: Container(
            alignment: Alignment.center,
            padding: EdgeInsets.all(context.dynamicHeight * 0.015),
            decoration: BoxDecoration(
              color: MyColors.primaryColor,
              shape: BoxShape.circle,
              border: Border.all(color: Colors.white, width: 6),
            ),
            child: SvgPicture.asset(AssetsConstant.edit),
          ),
        ),
      ],
    );
  }
}
