import os
import uuid
from PIL import Image
import google.generativeai as genai
from google.generativeai.types import GenerationConfig

API_KEY = 'AIzaSyAmkp_yXn_TFpQCI2tnowoaA44hpgkvoh8'  
os.environ['GOOGLE_API_KEY'] = API_KEY

genai.configure(api_key=API_KEY)

generation_config = GenerationConfig(
  max_output_tokens = 2048,
  temperature = 0.1,
  top_p = 0.9,
  top_k = 30,
  response_mime_type="text/plain"
)

def prep_image(image_path, max_width=1600, max_height=2300):
  file_extension=image_path.split('.')[-1]
  
  with Image.open(image_path) as img:
    img.thumbnail((max_width, max_height))
    temp_path = f'{uuid.uuid4()}.{file_extension}'
    img.save(temp_path)
  
  sample_file = genai.upload_file(path=temp_path, display_name="Diagram")
  os.remove(temp_path)
  return sample_file

def extract_text_from_image(image_file):
  try:
    model = genai.GenerativeModel(model_name="models/gemini-1.5-flash-8b")
    prompt = """
    - Bir OCR gibi çalış ve metnin tamamını çıkar. Metin dili Türkiye Türkçesi dir.
    - Eğer resim bir SORU değil ise sadece #NanTestNan# cevabını ver.
    - Eğer resim bir SORU ise şu kuralları uygula:    
      - Eğer çoktan seçmeli bir test sorusu ise ve şıkları var ise:
        - A) veya a) ile başlayan satırı tespit et. Bu satır şıkların başladığı satırdır. Bu satırın üstündeki tüm satırlar soru satırıdır.
        - Soru satırlarını ekle. 
        - Şıklar arasında, yani başlangıcı "A","B","C","D","E" boş satır bırakamadan şıkları ekle. Şıkları her zaman harften sonra bir adet kapalı parantez olacak şekilde yaz.
        - Kesinlikle doğru cevabı verme, sadece metni çıkar.
      - Eğer çoktan seçmeli bir test sorusu değil ise:
        - Sadece soru metnini çıkar ve en sonuna ##classic## yazısını ekle.
      
    - Yukarıdaki tüm şartların gerçekleştirdiğinden EMİN OL!
    """
    response = model.generate_content([image_file, prompt], generation_config=generation_config)

    return ''.join([part.text for part in response.parts]) if response.parts else None
  except genai.GenerateContentError as e:
    print(f"OCR içeriği oluşmadı: {str(e)}")
  except Exception as e:
    print(f"OCR servisi hat verdi: {str(e)}")
  return None

def get_text(file_path):
  try:
    sample_file = prep_image(file_path)
    text = extract_text_from_image(sample_file)
    if "#NanTestNan#" in text:
      raise ValueError("Resim bir test sorusu değil.")
    return text
  except FileNotFoundError:
    print(f"Soru resmi bulunamadı: {file_path}")
  except Exception as e:
    print(f"Bir hata oluştu: {e}")
  raise ValueError("OCR çözümleme yapamadı")