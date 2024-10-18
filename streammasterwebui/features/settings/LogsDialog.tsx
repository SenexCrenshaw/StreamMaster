import { memo, useState } from 'react';
import useGetLogNames from '@lib/smAPI/Logs/useGetLogNames';
import { BaseSettings } from './BaseSettings';
import SMPopUp from '@components/sm/SMPopUp';
import LogDisplay from './LogDisplay';
import { ScrollPanel } from 'primereact/scrollpanel';
import CopyButton from '@components/buttons/CopyButton';
import SaveButton from '@components/buttons/SaveButton';

const LogsDialog = () => {
  const query = useGetLogNames();
  const [logContent, setLogContent] = useState<string>('');

  const handleDownloadLog = (logName: string) => {
    if (!logContent) return;
    const blob = new Blob([logContent], { type: 'text/plain' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = logName; // Save the file as logName.txt
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link); // Clean up the DOM after download
  };

  if (query.isLoading) {
    return <div>Loading...</div>;
  }

  if (query.isError) {
    return <div>Error loading logs</div>;
  }

  return (
    <BaseSettings title="LOGS">
      <ScrollPanel style={{ height: '15vh', width: '100%' }}>
        <div className="sm-begin-stuff">
          {query.data?.map((logName) => (
            <div key={logName}>
              <div className="sm-w-10rem">
                <SMPopUp
                  contentWidthSize="10"
                  modal
                  modalCentered
                  title={logName}
                  buttonLabel={logName}
                  header={
                    <div className="flex w-12 gap-1 justify-content-end align-content-center">
                      <CopyButton value={logContent} openCopyWindow={false} iconFilled />
                      <SaveButton onClick={() => handleDownloadLog(logName)} tooltip={`Download ${logName}`} />
                    </div>
                  }
                >
                  <LogDisplay logName={logName} onDataChange={setLogContent} />
                </SMPopUp>
              </div>
            </div>
          ))}
        </div>
      </ScrollPanel>
    </BaseSettings>
  );
};

LogsDialog.displayName = 'LogsDialog';

export default memo(LogsDialog);
