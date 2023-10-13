import { memo, useCallback, useEffect, useState } from 'react';

import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import OKButton from '../buttons/OKButton';
import RefreshButton from '../buttons/RefreshButton';

type FileRefreshDialogProps = {
  readonly fileType: 'epg' | 'm3u';
  readonly inputInfoMessage?: string;
  readonly onRefreshFile: () => void;
};

const FileRefreshDialog = ({ fileType, inputInfoMessage, onRefreshFile }: FileRefreshDialogProps) => {
  const labelName = fileType.toUpperCase();

  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState<string | undefined>(undefined);

  useEffect(() => {
    setInfoMessage(inputInfoMessage);
  }, [inputInfoMessage]);

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setBlock(false);
  }, []);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header={'Refresh ' + labelName}
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex grid justify-content-center align-items-center w-full">
          <OKButton
            label={'Refresh ' + labelName}
            onClick={() => {
              setBlock(true);
              onRefreshFile();
            }}
          />
        </div>
      </InfoMessageOverLayDialog>

      <RefreshButton onClick={() => setShowOverlay(true)} tooltip={'Refresh ' + labelName} />
    </>
  );
};

export default memo(FileRefreshDialog);
