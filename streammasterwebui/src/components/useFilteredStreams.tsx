/* eslint-disable @typescript-eslint/no-explicit-any */
import { useState, useEffect } from 'react';
import type * as StreamMasterApi from '../store/iptvApi';
import React from 'react';

type QueryData = StreamMasterApi.VideoStreamDto[];
type ChannelGroupsQuery = StreamMasterApi.ChannelGroupDto[];
type Props = { groups?: StreamMasterApi.ChannelGroupDto[], m3uFileId?: number };


const getGroupNames = (channelGroupsQuery: ChannelGroupsQuery, propsGroupIds: Array<StreamMasterApi.ChannelGroupDto['id']>): Array<StreamMasterApi.ChannelGroupDto['name']> => {
  return channelGroupsQuery?.filter(
    (group) => propsGroupIds.includes(group.id) && group.name !== undefined
  ).map((group) => group.name) ?? [];
}

const addRegexMatchData = (channelGroupsQuery: ChannelGroupsQuery, groupNames: string[], data: QueryData, processedData: QueryData): QueryData => {
  groupNames.forEach((groupName: string) => {
    const cg = channelGroupsQuery?.find(
      (x: StreamMasterApi.ChannelGroupDto) =>
        x.name.toLowerCase() === groupName.toLowerCase()
    );

    if (cg?.regexMatch !== undefined && cg.regexMatch !== "") {
      const filteredData = data?.filter((item) => {
        if (item.isHidden || !item.user_Tvg_name) return false;
        const regexToTest = new RegExp(`.*${cg.regexMatch}.*`, "i");
        const test = regexToTest.test(item.user_Tvg_name);
        if (test) {
          return processedData.includes(item) === false;
        }

        return false;
      });

      if (filteredData !== undefined) {
        processedData = processedData.concat(filteredData);
      }
    }
  });

  return processedData;
}


const processDataWithGroups = (channelGroupsQuery: ChannelGroupsQuery, props: Props, data: QueryData): QueryData => {
  if (!data) {
    return [];
  }

  let propsGroupIds: Array<StreamMasterApi.ChannelGroupDto['id']> = [];

  if (props.groups) {
    propsGroupIds = props.groups.map(
      (group: StreamMasterApi.ChannelGroupDto) => group.id
    );
  }

  const groupNames = getGroupNames(channelGroupsQuery, propsGroupIds);

  let processedData = data.filter(
    (stream: StreamMasterApi.VideoStreamDto) =>
      stream.user_Tvg_group !== undefined && stream.user_Tvg_group !== null && groupNames.includes(stream.user_Tvg_group)
  );

  processedData = addRegexMatchData(channelGroupsQuery, groupNames, data, processedData);

  return processedData;
}


const processStreamsData = (channelGroupsQuery: ChannelGroupsQuery, props: Props, data: QueryData): QueryData => {
  if (!data) {
    return [];
  }

  if (
    !props.groups ||
    props.groups.length === 0 ||
    props.groups[0].name === undefined ||
    props.groups.findIndex(
      (group: StreamMasterApi.ChannelGroupDto) => group.name === "All"
    ) !== -1
  ) {
    return data;
  }

  return processDataWithGroups(channelGroupsQuery, props, data);
}


export const useFilteredStreams = (
  channelGroupsQuery: any,
  props: Props,
  videoStreamsQuery: any
): QueryData => {

  const isValidQueryData = (data?: QueryData): boolean => {
    return !!data && data.length > 0;
  }

  const [filteredStreams, setFilteredStreams] = useState<QueryData>([]);
  const processedData = React.useMemo(() => processStreamsData(channelGroupsQuery.data, props, videoStreamsQuery.data), [channelGroupsQuery.data, props, videoStreamsQuery.data]);

  useEffect(() => {
    if (!isValidQueryData(videoStreamsQuery.data)) {
      setFilteredStreams([]);
      return;
    }

    let data = processedData;

    if (props.m3uFileId && props.m3uFileId > 0) {
      data = data.filter(
        (stream: StreamMasterApi.VideoStreamDto) => stream.m3UFileId === props.m3uFileId
      );
    }

    setFilteredStreams(data);
  }, [props.m3uFileId, processedData, videoStreamsQuery.data]);

  return filteredStreams;
}

