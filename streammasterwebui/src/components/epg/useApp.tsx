/* eslint-disable @typescript-eslint/no-unused-vars */
/* eslint-disable @typescript-eslint/no-floating-promises */
/* eslint-disable @typescript-eslint/no-shadow */
import React from "react";

import { type Channel, type Program } from "planby";
import { useEpg } from "planby";
import { fetchChannels, fetchEpg } from "./common";
import * as StreamMasterApi from '../../store/iptvApi';
import { useMountEffect } from 'primereact/hooks';

// Example of globalStyles
// const globalStyles = `
// @import url('https://fonts.googleapis.com/css2?family=Antonio:wght@400;500;600&display=swap');
// .planby {
//   font-family: "Antonio", system-ui, -apple-system,
//     /* Firefox supports this but not yet system-ui */ "Segoe UI", Roboto,
//     Helvetica, Arial, sans-serif, "Apple Color Emoji", "Segoe UI Emoji"; /* 2 */
// }
// `;

export function useApp() {
  const [channels, setChannels] = React.useState<Channel[]>([]);
  const [epg, setEpg] = React.useState<Program[]>([]);
  const [isLoading, setIsLoading] = React.useState<boolean>(false);

  const epgForGuide = StreamMasterApi.useStreamGroupsGetStreamGroupEpgForGuideQuery(1);

  const channelsData = React.useMemo(() => channels, [channels]);
  const epgData = React.useMemo(() => epg, [epg]);

  const startDate = React.useMemo(() => {
    const sd = new Date();
    sd.setMinutes(sd.getMinutes() - 30);
    return sd;
  }, []);

  const endDate = React.useMemo(() => {
    const sd = new Date();
    sd.setHours(sd.getHours() + 2);
    return sd;
  }, []);

  React.useEffect(() => {
    if (epgForGuide.data) {
      setIsLoading(true);
      const chs = epgForGuide.data?.channels as Channel[];
      setChannels(chs);

      const sd = new Date();
      sd.setHours(sd.getHours() - 24);

      var ed = new Date();
      ed.setHours(ed.getHours() + 48);

      const ret = epgForGuide.data?.programs as Program[];
      const test = ret.filter((p) => new Date(p.since) >= sd && new Date(p.since) <= ed);

      setEpg(test);
      setIsLoading(false);
    }
  }, [epgForGuide.data]);


  const { getEpgProps, getLayoutProps } = useEpg({
    channels: channelsData,
    dayWidth: 1160,
    endDate: endDate,
    epg: epgData,
    isBaseTimeFormat: true,
    isLine: true,
    isSidebar: true,
    isTimeline: true,
    itemHeight: 80,
    sidebarWidth: 80,
    startDate: startDate
  });


  return { getEpgProps, getLayoutProps, isLoading };
}
