import { useState, useEffect } from 'react';

const useFetchConfiguration = () => {
    // Initial default values
    const [config, setConfig] = useState({
        baseHostURL: 'http://127.0.0.1:7095',
        apiRoot: '',
        apiKey: '',
        isDebug: false,
        urlBase: '',
        version: '',
    });

    useEffect(() => {
        async function fetchConfiguration() {
            try {
                let data = null;
                let sourceURL = '/initialize.js';

                // Try fetching from /initialize.js first
                let response = await fetch(sourceURL);

                if (!response.ok) {
                    // If unsuccessful, try fetching from config.baseHostURL + /initialize.js
                    sourceURL = `http://127.0.0.1:7095/initialize.js`;
                    response = await fetch(sourceURL);
                }

                if (response.ok) {
                    data = await response.json();
                    console.log('Setting config ', data);
                    setConfig({
                        baseHostURL: data.baseHostURL || config.baseHostURL,
                        apiRoot: data.apiRoot || config.apiRoot,
                        apiKey: data.apiKey || config.apiKey,
                        isDebug: data.isDebug || config.isDebug,
                        urlBase: data.urlBase || config.urlBase,
                        version: data.version || config.version,
                    });

                    console.log(`Successfully fetched configuration from ${sourceURL}`);
                } else {
                    console.error('Failed to fetch configuration from both endpoints.');
                }

            } catch (error) {
                console.error('Error fetching configuration:', error);
            }
        }

        // Fetch configuration when the hook is first used
        fetchConfiguration();
    }, []);

    return config;
};

export default useFetchConfiguration;
