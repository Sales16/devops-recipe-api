from database import Base, engine
from models import Ingrediente

try:
    Base.metadata.create_all(bind=engine)
    print("Banco criado com sucesso!")
except Exception as e:
    print(f"Erro ao criar o banco de dados: {e}")
