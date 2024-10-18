import { memo, useMemo } from 'react';
import useGetLogContents from '@lib/smAPI/Logs/useGetLogContents';
import { ScrollPanel } from 'primereact/scrollpanel';
import { JsonEditor } from 'json-edit-react';

interface LogDisplayProperties {
  readonly logName: string;
}

const LogDisplay = ({ logName }: LogDisplayProperties) => {
  const query = useGetLogContents({ logName });

  const parsedJsonString = query.data
    .replace(/\\u0022/g, '"') // Replace unicode for double quotes
    .replace(/\\r\\n/g, '\r\n') // Replace escaped newlines
    .replace(/\\\\/g, '\\'); // Replace escaped backslashes

  const getContent = useMemo(() => {
    if (!query.data) return <div>Loading...</div>;
    let jsonObject = JSON.parse(query.data);
    return <JsonEditor data={jsonObject} restrictEdit restrictDelete restrictAdd theme="githubDark" />;
  }, [query.data]);

  if (query?.data === undefined || query.isLoading) {
    return <div>Loading...</div>;
  }

  if (query.isError) {
    return <div>Error loading logs</div>;
  }

  return (
    <ScrollPanel style={{ height: '80vh', width: '100%' }}>
      <pre style={{ whiteSpace: 'pre-wrap', wordWrap: 'break-word' }}>{parsedJsonString}</pre>
    </ScrollPanel>
  );
};

LogDisplay.displayName = 'LogDisplay';

export default memo(LogDisplay);
