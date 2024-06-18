import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { RemoveFileProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { FileOutputProfileDto, RemoveFileProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef } from 'react';

interface RemoveFileProfileDialogProps {
  fileOutputProfileDto: FileOutputProfileDto;
}

const RemoveFileProfileDialog = ({ ...props }: RemoveFileProfileDialogProps) => {
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const remove = useCallback(() => {
    const request = {
      Name: props.fileOutputProfileDto.Name
    } as RemoveFileProfileRequest;

    RemoveFileProfile(request)
      .then((response) => {})
      .catch((error) => {
        Logger.error('RemoveFileProfileDialog', { error });
      })
      .finally(() => {
        smPopUpRef.current?.hide();
      });
  }, [props]);

  return (
    <SMPopUp
      contentWidthSize="2"
      buttonDisabled={props.fileOutputProfileDto.IsReadOnly}
      buttonClassName="icon-red"
      icon="pi-times"
      info=""
      title="Remove Profile?"
      modal
      modalCentered
      onOkClick={() => {
        remove();
      }}
      ref={smPopUpRef}
      okButtonDisabled={!props.fileOutputProfileDto.Name}
      tooltip="Remove Profile"
      zIndex={10}
    >
      <div className="sm-center-stuff">
        <div className="text-container"> {props.fileOutputProfileDto.Name}</div>
      </div>
    </SMPopUp>
  );
};

export default RemoveFileProfileDialog;
