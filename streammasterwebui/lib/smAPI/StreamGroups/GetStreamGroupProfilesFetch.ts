import { GetStreamGroupProfiles } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetStreamGroupProfiles = createAsyncThunk('cache/getGetStreamGroupProfiles', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetStreamGroupProfiles');
    const response = await GetStreamGroupProfiles();
    console.log('Fetched GetStreamGroupProfiles ',response?.length);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


