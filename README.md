# PruebaTecnica

Para correr la aplicacion puedes hacer uso del comando 
docker-compose up -d

Esto levanta dos instancias de la aplicacion

Tienes dos endpoints:
![image](https://github.com/user-attachments/assets/7a0b276d-6c42-4b9a-ae75-7807a20a3b92)

El primero le permite consumir un API de paises, como queryparam se tiene la cantidad de segundos deseado en los cuales se desea que esta guardado los datos en cache (en caso de que sea por primera vez o el cache este invalidado)

El segundo endpoint le permite invalidar ese cache para que pruebe de nuevo a consumir la API, el response del primer endpoint te da datos como el origen del cual obtuvo los datos y el tiempo que se tardó en consumir estos datos
