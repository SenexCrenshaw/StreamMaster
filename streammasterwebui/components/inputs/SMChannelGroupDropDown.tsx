import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, forwardRef, memo, useMemo, useRef } from 'react';

import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';

interface SMChannelGroupDropDownProperties {
  readonly smChannelDto: SMChannelDto;
  readonly darkBackGround?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange: (value: string) => void;
}

const SMChannelGroupDropDown = forwardRef<SMDropDownRef, SMChannelGroupDropDownProperties>((props: SMChannelGroupDropDownProperties, ref) => {
  const { darkBackGround, smChannelDto, onChange, label, labelInline = false } = props;
  const smDropownRef = useRef<SMDropDownRef>(null);
  const { data } = useGetChannelGroups();

  const itemTemplate = (option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return (
      <div className="border-12">
        <div className="text-container">{option.Name}</div>
      </div>
    );
  };

  const buttonTemplate = useMemo((): ReactNode => {
    if (!smChannelDto || !smChannelDto.Group) {
      return <div className="text-xs text-container text-white-alpha-40 pl-1">None</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{smChannelDto.Group}</div>
      </div>
    );
  }, [smChannelDto]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditor ';

    if (label && !labelInline) {
      ret += '';
    }

    if (labelInline) {
      ret += ' align-items-start';
    } else {
      ret += ' align-items-center';
    }

    return ret;
  }, [label, labelInline]);

  return (
    <>
      {label && !labelInline && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
        </>
      )}
      <div className={getDiv}>
        {label && labelInline && <div className={labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
        <SMDropDown
          buttonLabel="GROUP"
          buttonDarkBackground={darkBackGround}
          buttonTemplate={buttonTemplate}
          closeOnSelection
          data={data}
          dataKey="Group"
          filter
          filterBy="Name"
          itemTemplate={itemTemplate}
          onChange={(e) => {
            onChange(e.Name);
          }}
          ref={smDropownRef}
          title="GROUP"
          value={smChannelDto}
          optionValue="Name"
          contentWidthSize="2"
        />
      </div>
    </>
  );
});

export default memo(SMChannelGroupDropDown);
