
import { Dropdown } from 'primereact/dropdown';
import { type SelectItem } from 'primereact/selectitem';
import React from 'react';
import { useStreamGroupsGetStreamGroupsQuery, type StreamGroupDto } from '../store/iptvApi';

export const StreamGroupSelector =
  React.forwardRef((props: StreamGroupSelectorProps, ref) => {

    const elementRef = React.useRef(null);

    React.useImperativeHandle(ref, () => ({
      getElement: () => elementRef.current,
      props
    }));


    const [selectedStreamGroup, setSelectedStreamGroup] = React.useState<StreamGroupDto>({} as StreamGroupDto);

    const streamGroups = useStreamGroupsGetStreamGroupsQuery();

    const isDisabled = React.useMemo((): boolean => {
      if (streamGroups.isLoading) {
        return true;
      }

      return false;
    }, [streamGroups.isLoading]);


    const onDropdownChange = (sg: StreamGroupDto) => {
      if (!sg) return;

      setSelectedStreamGroup(sg);
      props?.onChange?.(sg);
    };

    const getOptions = React.useMemo((): SelectItem[] => {
      if (!streamGroups.data)
        return [
          {
            label: 'Loading...',
            value: {} as StreamGroupDto,
          } as SelectItem,
        ];

      const ret = streamGroups.data?.map((a) => {
        return { label: a.name, value: a } as SelectItem;
      });

      ret.unshift({
        label: 'All',
        value: {} as StreamGroupDto,
      } as SelectItem);

      return ret;
    }, [streamGroups.data]);

    return (<div  >
      <Dropdown
        className="streamgroupselector"
        disabled={isDisabled}
        onChange={(e) => onDropdownChange(e.value)}
        options={getOptions}
        placeholder="Stream Group"
        ref={elementRef}
        value={selectedStreamGroup}
      />
    </div>
    );
  });
StreamGroupSelector.displayName = 'StreamGroupSelector';
StreamGroupSelector.defaultProps = {

};
type StreamGroupSelectorProps = {
  onChange: ((value: StreamGroupDto) => void);
};
