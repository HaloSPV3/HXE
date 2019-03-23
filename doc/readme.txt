DOMAIN DOCUMENTATION
====================

This section serves as documentation for the fundamental domain entities
& logic of SPV3. For documentation on the implementations & source code,
please review the readme.txt file in the src directory.

SPV3 INFORMATION
----------------

The following table outlines the documentation focusing on information
specific to the SPV3 programs:

  ---------------------------------------------------------------------
  Documentation                         Description
  ------------------------------------- -------------------------------
  loader.txt                            Outlines the loading procedure,
                                        including asset verification,
                                        campaign resuming,
                                        configuration overrides and
                                        loading SPV3.

  shaders.txt                           Specification on the
                                        user-configurable
                                        post-processing effects, and
                                        saving + loading settings to
                                        and from the initc.txt

  release.txt                           Documentation on the public
                                        release procedure of SPV3.2,
                                        including distribution &
                                        installation decisions.

  overridex.txt                         Documentation & instructions on
                                        overriding configurations, for
                                        debugging purposes.
  ---------------------------------------------------------------------

FILESYSTEM FILES
----------------

The following table outlines the documentation focusing on files on the
filesystem that SPV3.2 deals with:

  ---------------------------------------------------------------------
  Documentation                         Description
  ------------------------------------- -------------------------------
  initc.txt                             Documentation on the initc.txt
                                        file, which is used for
                                        declaring global variables that
                                        SPV3.2 must load.

  savegame.txt                          Documentation on the
                                        savegame.bin file, which is
                                        used for resuming the campaign.

  lastprof.txt                          Documentation on the
                                        lastprof.txt, which is used for
                                        inferring the last used profile
                                        name.

  opensauce.txt                         Documentation on OpenSauce,
                                        which SPV3 depends on for
                                        post-processing and other
                                        goodies.
  ---------------------------------------------------------------------
