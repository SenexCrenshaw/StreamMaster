import { memo } from 'react';
import useGetLogNames from '@lib/smAPI/Logs/useGetLogNames';
import { BaseSettings } from './BaseSettings';
import { Logger } from '@lib/common/logger';
import SMPopUp from '@components/sm/SMPopUp';
import LogDisplay from './LogDisplay';

const LogsDialog = () => {
  const query = useGetLogNames();
  Logger.debug('LogsDialog', 'query', query.data);
  if (query.isLoading) {
    return <div>Loading...</div>;
  }

  if (query.isError) {
    return <div>Error loading logs</div>;
  }

  return (
    <BaseSettings title="LOGS">
      <div className="sm-begin-stuff">
        {query.data?.map((logName) => (
          <div key={logName}>
            <div className="sm-w-10rem">
              <SMPopUp contentWidthSize="10" modal modalCentered title={logName} buttonLabel={logName}>
                <LogDisplay logName={logName} />
              </SMPopUp>
            </div>
          </div>
        ))}
      </div>
    </BaseSettings>
  );
};

LogsDialog.displayName = 'LogsDialog';

export default memo(LogsDialog);
