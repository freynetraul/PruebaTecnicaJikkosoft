# Prueba Técnica Jikkosoft

Este proyecto implementa una API simple que consume datos de países y los almacena en caché.  La aplicación se puede ejecutar fácilmente usando Docker Compose.

## Requisitos

*   [Docker](https://www.docker.com/)
*   [Docker Compose](https://docs.docker.com/compose/)

## Instalación y Ejecución

1.  **Clonar el repositorio:**

    ```bash
    git clone https://github.com/freynetraul/PruebaTecnicaJikkosoft.git
    cd PruebaTecnicaJikkosoft/PruebaTecnica
    ```

2.  **Ejecutar la aplicación con Docker Compose:**

    ```bash
    docker-compose up -d
    ```

    Este comando levantará dos instancias de la aplicación en segundo plano.

Redirigete a 
  ```bash
[UrlUrlInstancia1]//swagger/index.html
  ```
y 
  ```bash
[UrlInstancia2]//swagger/index.html
  ```


Tienes dos endpoints:

![image](https://github.com/user-attachments/assets/7a0b276d-6c42-4b9a-ae75-7807a20a3b92)

El primero le permite consumir un API de paises, como queryparam se tiene la cantidad de segundos deseado en los cuales se desea que esta guardado los datos en cache (en caso de que sea por primera vez o el cache este invalidado)

El segundo endpoint le permite invalidar ese cache para que pruebe de nuevo a consumir la API, el response del primer endpoint te da datos como el origen del cual obtuvo los datos y el tiempo que se tardó en consumir estos datos

Si prefieres puedes ejecutar directamente el endpoint en el navegador o postman asi:

  ```bash
[UrlInstancia]/api/Test?DurationCache=[TiempoEnSegundosDuraccionCache]
  ```

  
