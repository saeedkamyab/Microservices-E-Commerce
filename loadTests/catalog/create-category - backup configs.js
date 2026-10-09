import http from 'k6/http';
import { check } from 'k6';

export const options = {
    vus: 500,
    duration: '160s',
    insecureSkipTLSVerify: true,
};

export default function () {

    // Create a unique ID for each request
    const id = `${__VU}-${__ITER}`;

    // Request body
    const payload = JSON.stringify({
        name: `LoadTest-Category-${id}`,
        parentCategoryId: null
    });

    // Send POST request
    const response = http.post(
        'http://localhost:5000/api/catalog/categories',
        payload,
        {
            headers: {
                'Content-Type': 'application/json'
            }
        },
		   {
    timeout: '1600s'
}
    );

    // Check response status
    const success = check(response, {
        'status is 2xx': (r) =>
            r.status >= 200 && r.status < 300
    });

    // Only print failed requests
    if (!success) {
        console.log(
            `Request failed | VU=${__VU} | ITER=${__ITER} | STATUS=${response.status}`
        );

   console.log(`Error: ${response.error}`);
    console.log(`Error code: ${response.error_code}`);
   
        console.log(`Response: ${response.body}`);
    }
}