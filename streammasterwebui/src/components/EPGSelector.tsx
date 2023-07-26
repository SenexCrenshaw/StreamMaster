import * as React from 'react';
import * as StreamMasterApi from '../store/iptvApi';
import * as Hub from "../store/signlar_functions";
import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import DropDownEditorBodyTemplate from './DropDownEditorBodyTemplate';

const EPGSelector = (props: EPGSelectorProps) => {
  const toast = React.useRef<Toast>(null);
  const [programme, setProgramme] = React.useState<string>('');
  const [dataDataSource, setDataSource] = React.useState<StreamMasterApi.ProgrammeName[]>([]);

  const programmeNamesQuery = StreamMasterApi.useProgrammesGetProgrammeNamesQuery();


  React.useEffect(() => {
    if (!programmeNamesQuery.data) {
      return;
    }

    const newData = [...programmeNamesQuery.data];
    newData.unshift({ displayName: '<Blank>' });
    setDataSource(newData);

    return () => {
      setDataSource([]);
    };

  }, [programmeNamesQuery.data]);

  React.useEffect(() => {
    if (!dataDataSource) {
      return;
    }

    if (props.value === null || props.value === undefined) {
      setProgramme('Dummy');
      return;
    }

    setProgramme(props.value);

  }, [dataDataSource, props.value]);

  const onEPGChange = React.useCallback(async (channel: string) => {

    if (channel === undefined) {
      return;
    }

    setProgramme(channel);

    if (props.data === undefined || props.data.id === '' || props.data.user_Tvg_ID === channel) {
      return;
    }

    if (props.onChange) {
      props.onChange(channel);
    }

    const data = {} as StreamMasterApi.UpdateVideoStreamRequest;
    data.tvg_ID = channel;
    data.id = props.data.id;
    if (channel === '<Blank>') {
      channel = '';
    }

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

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  if (props.enableEditMode !== true) {

    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {programme !== '' ? programme : 'Dummy'}
      </div>
    )

  }

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="iconSelector flex w-full justify-content-center align-items-center">

        <DropDownEditorBodyTemplate
          className={className}
          data={dataDataSource.map((a) => (a.displayName ?? ''))}
          onChange={async (e) => {
            await onEPGChange(e);
          }}
          value={programme}
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
  value?: string | null;
};

export default React.memo(EPGSelector);
