import { useStreamGroupsCreateStreamGroupMutation, type CreateStreamGroupRequest } from '@lib/iptvApi';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import InfoMessageOverLayDialog from '../InfoMessageOverLayDialog';
import AddButton from '../buttons/AddButton';
import TextInput from '../inputs/TextInput';

const StreamGroupAddDialog = (props: StreamGroupAddDialogProperties) => {
  const [showOverlay, setShowOverlay] = useState<boolean>(false);
  const [block, setBlock] = useState<boolean>(false);
  const [infoMessage, setInfoMessage] = useState('');
  const [name, setName] = useState<string>('');

  const [streamGroupsCreateStreamGroupMutation] = useStreamGroupsCreateStreamGroupMutation();

  const ReturnToParent = useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setName('');
    setBlock(false);
    props.onHide?.();
  }, [props]);

  const isSaveEnabled = useMemo((): boolean => {
    if (name && name !== '') {
      return true;
    }

    return false;
  }, [name]);

  const onAdd = useCallback(() => {
    setBlock(true);

    if (!isSaveEnabled) {
      ReturnToParent();
      return;
    }

    const data = {} as CreateStreamGroupRequest;
    data.name = name;

    streamGroupsCreateStreamGroupMutation(data)
      .then(() => {
        setInfoMessage('Stream Group Added Successfully');
      })
      .catch((error) => {
        setInfoMessage(`Stream Group Add Error: ${error.message}`);
      });
  }, [ReturnToParent, isSaveEnabled, name, streamGroupsCreateStreamGroupMutation]);

  useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();

        if (name !== '') {
          onAdd();
        }
      }
    };

    document.addEventListener('keydown', callback);

    return () => {
      document.removeEventListener('keydown', callback);
    };
  }, [onAdd, name]);

  return (
    <>
      <InfoMessageOverLayDialog
        blocked={block}
        closable
        header="Add Stream Group"
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >
        <div className="flex grid justify-content-center align-items-center w-full">
          <div className="flex col-10 mt-1">
            <TextInput label="Name" onChange={setName} value={name} />
          </div>
          <div className="flex col-12 justify-content-end">
            <AddButton disabled={!isSaveEnabled} label="Add Stream Group" onClick={() => onAdd()} tooltip="Add Stream Group" />
          </div>
        </div>
      </InfoMessageOverLayDialog>

      <AddButton onClick={() => setShowOverlay(true)} tooltip="Add Stream Group" />
    </>
  );
};

StreamGroupAddDialog.displayName = 'StreamGroupAddDialog';

interface StreamGroupAddDialogProperties {
  readonly onHide?: () => void;
}

export default memo(StreamGroupAddDialog);
