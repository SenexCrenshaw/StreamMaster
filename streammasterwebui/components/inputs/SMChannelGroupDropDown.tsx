import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, forwardRef, memo, useEffect, useMemo, useRef, useState } from 'react';

import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';
import useIsRowLoading from '@lib/redux/hooks/useIsRowLoading';
import ChannelGroupAddDialog from '@components/channelGroups/ChannelGroupAddDialog';

interface SMChannelGroupDropDownProperties {
  readonly autoPlacement?: boolean;
  readonly darkBackGround?: boolean;
  readonly isLoading?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange: (value: string) => void;
  readonly smChannelDto?: SMChannelDto;
  readonly value?: string;
}

const SMChannelGroupDropDown = forwardRef<SMDropDownRef, SMChannelGroupDropDownProperties>((props: SMChannelGroupDropDownProperties, ref) => {
  const { darkBackGround, smChannelDto, autoPlacement = false, onChange, label, labelInline = false, value } = props;

  const smDropownRef = useRef<SMDropDownRef>(null);

  const { data, isLoading } = useGetChannelGroups();
  const [channelGroup, setChannelGroup] = useState<ChannelGroupDto | null>(null);

  const [isRowLoading] = useIsRowLoading({ Entity: 'SMChannel', Id: channelGroup?.Id?.toString() ?? '' });

  useEffect(() => {
    if (!data) {
      return;
    }

    if (smChannelDto !== undefined && (channelGroup === null || channelGroup.Name !== smChannelDto.Group)) {
      const found = data.find((predicate) => predicate.Name === smChannelDto.Group);
      if (found) setChannelGroup(found);
      return;
    }

    if (value !== undefined && (channelGroup === null || channelGroup.Name !== value)) {
      const found = data.find((predicate) => predicate.Name === value);
      if (found) setChannelGroup(found);
      return;
    }
  }, [channelGroup, data, smChannelDto, value]);

  const itemTemplate = (option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return <div className="text-container">{option.Name}</div>;
  };

  const buttonTemplate = useMemo((): ReactNode => {
    if (!channelGroup) {
      return <div className="text-xs text-container text-white-alpha-40 pl-1">Dummy</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{channelGroup.Name}</div>
      </div>
    );
  }, [channelGroup]);

  const headerRightTemplate = useMemo(() => <ChannelGroupAddDialog />, []);

  const getDiv = useMemo(() => {
    let ret = 'stringedito';

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
          autoPlacement={autoPlacement}
          buttonDarkBackground={darkBackGround}
          buttonLabel="GROUP"
          buttonTemplate={buttonTemplate}
          closeOnSelection
          data={data}
          dataKey="Name"
          filter
          filterBy="Name"
          info=""
          header={headerRightTemplate}
          buttonIsLoading={isLoading || props.isLoading || isRowLoading}
          itemTemplate={itemTemplate}
          onChange={(e) => {
            onChange(e.Name);
          }}
          ref={smDropownRef}
          title="GROUP"
          value={channelGroup}
          contentWidthSize="2"
          // zIndex={10}
        />
      </div>
    </>
  );
});

export default memo(SMChannelGroupDropDown);
