import http from 'k6/http';
import { check } from 'k6';

export const options = {
    vus: 300,
    duration: '60s',
    insecureSkipTLSVerify: true,
};

export default function () {

    // Create a unique ID for each request
    const id = `${__VU}-${__ITER}`;

    // Request body
    const payload = JSON.stringify({
        name: `LoadTest-Category-${id}`,

        parentCategoryId: null,

        attributes: [
            {
                name: `RAM-${id}`,
                type: 2,
                isRequired: true,
                options: []
            },
            {
                name: `5G-${id}`,
                type: 4,
                isRequired: false,
                options: []
            }
        ]
    });

    // Send POST request
    const response = http.post(
        'http://localhost:5000/api/catalog/categories',
        payload,
        {
            headers: {
                'Content-Type': 'application/json'
            }
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

        console.log(`Response: ${response.body}`);
    }
}