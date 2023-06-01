import { type DropdownProps } from 'primereact/dropdown';
import { Dropdown } from 'primereact/dropdown';
import * as React from 'react';
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';

const EPGSelector = (props: EPGSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [programme, setProgramme] = React.useState<StreamMasterApi.ProgrammeName>({} as StreamMasterApi.ProgrammeName);
  const programmeNamesQuery = StreamMasterApi.useProgrammesGetProgrammeNamesQuery();

  React.useMemo(() => {
    if (!programmeNamesQuery.data) {
      return;
    }

    let foundProgramme = {} as StreamMasterApi.ProgrammeName | undefined;

    if (props.value !== '') {
      foundProgramme = programmeNamesQuery.data.find((a) => a.channel === props.value);
    }

    if (foundProgramme?.channel !== undefined) {
      setProgramme(foundProgramme);
      return;
    }

    setProgramme({} as StreamMasterApi.ProgrammeName);

  }, [programmeNamesQuery, props.value]);

  const epgOptionTemplate = React.useCallback((programmeName: StreamMasterApi.ProgrammeName) => {
    return (
      <span className="text-sm" >{programmeName.displayName !== '' ? programmeName.displayName : programmeName.channelName}</span>
    );
  }, []);

  const onEPGChange = React.useCallback(async (channel: StreamMasterApi.ProgrammeName) => {

    if (channel === undefined || channel.channel === undefined) {
      return;
    }

    setProgramme(channel);

    if (props.data === undefined || props.data.id < 0 || props.data.user_Tvg_ID === channel.channel) {
      return;
    }

    if (props.onChange) {
      props.onChange(channel.channel);
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.id = props.data.id;
    data.tvg_ID = channel.channel;

    await Hub.UpdateVideoStream(data)
      .then((result) => {
        if (toast.current) {
          if (result) {
            toast.current.show({
              detail: `Updated Stream`,
              life: 3000,
              severity: 'success',
              summary: 'Successful',
            });
          } else {
            toast.current.show({
              detail: `Update Stream Failed`,
              life: 3000,
              severity: 'error',
              summary: 'Error',
            });
          }

        }
      }).catch((e) => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error ' + e.message,
          });
        }
      });



  }, [props]);

  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  const selectedTemplate = React.useCallback((option: any, propss: DropdownProps) => {

    if (propss.value === null || propss.value === undefined || propss.value.channelName === null || propss.value.channelName === undefined) {
      return <span>{propss.placeholder}</span>;
    }

    const programmeName = propss.value as StreamMasterApi.ProgrammeName;

    if (programmeName === undefined || programmeName.channel === undefined || programmeName.channelName === undefined) {
      return <span>{propss.placeholder}</span>;
    }

    return (
      <div className='flex h-full justify-content-start align-items-center p-0 m-0 pl-2'>
        {programmeName.displayName !== '' ? programmeName.displayName : programmeName.channelName}
      </div>
    );

  }, []);

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  if (props.enableEditMode !== true) {

    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {programme.displayName !== '' ? programme.displayName : programme.channelName}
      </div>
    )

  }

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="iconSelector flex w-full justify-content-center align-items-center">
        <Dropdown
          className={className}
          filter
          filterBy='channelName'
          itemTemplate={epgOptionTemplate}
          onChange={async (e) => { await onEPGChange(e.value); }}
          onClick={props.onClick}
          optionLabel='channel'
          options={programmeNamesQuery.data}
          placeholder="No EPG"
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
          value={programme}
          valueTemplate={selectedTemplate}
          virtualScrollerOptions={{
            itemSize: 32,
          }}
        />
      </div>
    </>
  );
};

EPGSelector.displayName = 'EPGSelector';
EPGSelector.defaultProps = {
  className: null,
  enableEditMode: true,
  onChange: null,
  value: null,
};

type EPGSelectorProps = {
  className?: string | null;
  data?: StreamMasterApi.VideoStreamDto | undefined;
  enableEditMode?: boolean;
  onChange?: ((value: string) => void) | null;
  onClick?: React.MouseEventHandler | undefined;
  value?: string | null;
};

export default React.memo(EPGSelector);
