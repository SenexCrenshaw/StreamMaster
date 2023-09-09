import { InputText } from "primereact/inputtext";
import React from "react";
import { useStreamGroupsCreateStreamGroupMutation, type CreateStreamGroupRequest } from "../../store/iptvApi";
import InfoMessageOverLayDialog from "../InfoMessageOverLayDialog";
import AddButton from "../buttons/AddButton";

const StreamGroupAddDialog = (props: StreamGroupAddDialogProps) => {
  const [showOverlay, setShowOverlay] = React.useState<boolean>(false);
  const [block, setBlock] = React.useState<boolean>(false);
  const [infoMessage, setInfoMessage] = React.useState('');
  const [name, setName] = React.useState<string>('');

  const [streamGroupsCreateStreamGroupMutation] = useStreamGroupsCreateStreamGroupMutation();

  const ReturnToParent = React.useCallback(() => {
    setShowOverlay(false);
    setInfoMessage('');
    setName('');
    setBlock(false);
    props.onHide?.();
  }, [props]);


  const isSaveEnabled = React.useMemo((): boolean => {

    if (name && name !== '') {
      return true;
    }

    // if (streamGroupNumber !== undefined && streamGroupNumber !== 0) {
    //   return true;
    // }

    return false;

  }, [name]);


  const onAdd = React.useCallback(() => {
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
      }).catch((e) => {
        setInfoMessage('Stream Group Add Error: ' + e.message);
      });
  }, [ReturnToParent, isSaveEnabled, name, streamGroupsCreateStreamGroupMutation]);

  React.useEffect(() => {
    const callback = (event: KeyboardEvent) => {
      if (event.code === 'Enter' || event.code === 'NumpadEnter') {
        event.preventDefault();

        if (name !== "") {
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
        header='Add Stream Group'
        infoMessage={infoMessage}
        onClose={() => {
          ReturnToParent();
        }}
        show={showOverlay}
      >

        <div className="flex grid justify-content-between align-items-center">
          <div className="flex col-12">
            <label className="col-2 " htmlFor="Name">Name: </label>
            <div className="col-8 ">
              <InputText
                autoFocus
                className='bordered-text-large'
                id="Name"
                onChange={(e) => setName(e.target.value)}
                type="text"
                value={name}
              />
            </div>
          </div>
          <div className="flex col-12 mt-3 gap-2 justify-content-end">
            <AddButton disabled={isSaveEnabled} label='Add Stream Group' onClick={() => onAdd()} tooltip='Add Stream Group' />
          </div>

        </div>
      </InfoMessageOverLayDialog >

      <AddButton onClick={() => setShowOverlay(true)} tooltip='Add Stream Group' />

    </>
  );
}

StreamGroupAddDialog.displayName = 'StreamGroupAddDialog';
StreamGroupAddDialog.defaultProps = {
}

type StreamGroupAddDialogProps = {
  readonly onHide?: () => void;
};

export default React.memo(StreamGroupAddDialog);
