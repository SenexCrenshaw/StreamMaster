import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { RemoveCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { CommandProfileDto, RemoveCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef } from 'react';

interface RemoveCommandProfileDialogProps {
  commandProfileDto: CommandProfileDto;
}

const RemoveCommandProfileDialog = ({ ...props }: RemoveCommandProfileDialogProps) => {
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const remove = useCallback(() => {
    const request = {
      ProfileName: props.commandProfileDto.ProfileName
    } as RemoveCommandProfileRequest;

    RemoveCommandProfile(request)
      .then((response) => {})
      .catch((error) => {
        Logger.error('RemoveCommandProfileDialog', { error });
      })
      .finally(() => {
        smPopUpRef.current?.hide();
      });
  }, [props]);

  return (
    <SMPopUp
      contentWidthSize="2"
      buttonDisabled={props.commandProfileDto.IsReadOnly}
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
      okButtonDisabled={!props.commandProfileDto.ProfileName}
      tooltip="Remove Profile"
      zIndex={12}
    >
      <div className="sm-center-stuff">
        <div className="text-container"> {props.commandProfileDto.ProfileName}</div>
      </div>
    </SMPopUp>
  );
};

export default RemoveCommandProfileDialog;
