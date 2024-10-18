import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { RemoveOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { OutputProfileDto, RemoveOutputProfileRequest } from '@lib/smAPI/smapiTypes';

import { useCallback, useRef } from 'react';

interface RemoveOutputProfileDialogProps {
  outputProfileDto: OutputProfileDto;
}

const RemoveOutputProfileDialog = ({ ...props }: RemoveOutputProfileDialogProps) => {
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const remove = useCallback(() => {
    const request = {
      Name: props.outputProfileDto.ProfileName
    } as RemoveOutputProfileRequest;

    RemoveOutputProfile(request)
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
      buttonDisabled={props.outputProfileDto.IsReadOnly}
      buttonClassName="icon-red"
      icon="pi-times"
      info=""
      title="Remove Profile?"
      modal
      placement="bottom-end"
      onOkClick={() => {
        remove();
      }}
      ref={smPopUpRef}
      okButtonDisabled={!props.outputProfileDto.ProfileName}
      tooltip="Remove Profile"
      zIndex={11}
    >
      <div className="sm-center-stuff">
        <div className="text-container"> {props.outputProfileDto.ProfileName}</div>
      </div>
    </SMPopUp>
  );
};

export default RemoveOutputProfileDialog;
